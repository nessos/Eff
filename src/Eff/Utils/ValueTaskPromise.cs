namespace Nessos.Effects.Utils;

using System.Collections.Concurrent;
using System.Threading.Tasks.Sources;

// Defines a simple promise class for ValueTask which can be awaited multiple times.
// Adapted from Konrad Kokosa's IValueTaskSource example in
// http://tooslowexception.com/implementing-custom-ivaluetasksource-async-without-allocations/

internal sealed class ValueTaskPromise : IValueTaskSource
{
    private struct ContinuationContext
    {
        public Action<object>? Continuation;
        public object? State;
        public ExecutionContext? ExecutionContext;
        public object? Scheduler;
    }

    private volatile int _isTaskCompleted;
    private volatile int _subscriptionsInProgress;
    private volatile int _isFirstContinuationQueued;
    private ContinuationContext _firstContinuation;
    private ConcurrentQueue<ContinuationContext>? _additionalContinuations;

    public ValueTask Task => new(this, 0);

    // Signal task completion
    public void SetCompleted()
    {
        // First, mark task as completed or exit if already done.
        if (Interlocked.CompareExchange(ref _isTaskCompleted, 1, 0) == 1)
        {
            return;
        }

        // Spin until any pending continuation subscriptions have completed.
        while (_subscriptionsInProgress > 0)
        {
            Thread.SpinWait(20);
        }

        // Now, execute all queued continuations.
        if (_isFirstContinuationQueued == 1)
        {
            ExecuteContinuation(in _firstContinuation, isAsyncExecution: true);
            _firstContinuation = default;
        }

        if (_additionalContinuations != null)
        {
            while (_additionalContinuations.TryDequeue(out ContinuationContext ctx))
            {
                ExecuteContinuation(in ctx, isAsyncExecution: true);
            }
        }
    }

    // Handle continuations subscribed by awaiters
    void IValueTaskSource.OnCompleted(Action<object> continuation, object state, short _, ValueTaskSourceOnCompletedFlags flags)
    {
        ContinuationContext ctx = new() { Continuation = continuation, State = state };
        CaptureThreadContext(ref ctx, flags);

        // Coordinate with concurrent SetCompleted() calls:
        // Increment the subscriptionsInProgress count and read the task state, in that order.
        Interlocked.Increment(ref _subscriptionsInProgress);

        if (_isTaskCompleted == 1)
        {
            // Task completed, execute the continuation immediately.
            Interlocked.Decrement(ref _subscriptionsInProgress);
            ExecuteContinuation(in ctx, isAsyncExecution: false);
        }
        else
        {
            // Enqueue the continuation for asynchronous execution.
            if (Interlocked.CompareExchange(ref _isFirstContinuationQueued, 1, 0) == 0)
            {
                // this is the first continuation, store directly to field.
                _firstContinuation = ctx;
            }
            else
            {
                // this is a secondary continuation, add to a heap allocated queue.
                ConcurrentQueue<ContinuationContext> contQueue = GetOrCreateAdditionalContinuationQueue();
                contQueue.Enqueue(ctx);
            }

            // signal that the continuation enqueue operation has completed
            Interlocked.Decrement(ref _subscriptionsInProgress);
        }
    }

    ValueTaskSourceStatus IValueTaskSource.GetStatus(short token) => (_isTaskCompleted == 1) ? ValueTaskSourceStatus.Succeeded : ValueTaskSourceStatus.Pending;
    void IValueTaskSource.GetResult(short token) { }

    private ConcurrentQueue<ContinuationContext> GetOrCreateAdditionalContinuationQueue()
    {
        ConcurrentQueue<ContinuationContext>? contQueue = _additionalContinuations;
        if (contQueue == null)
        {
            contQueue = new ConcurrentQueue<ContinuationContext>();
            if (Interlocked.CompareExchange(ref _additionalContinuations, contQueue, null) is ConcurrentQueue<ContinuationContext> existing)
            {
                contQueue = existing;
            }
        }

        return contQueue;
    }

    private static void CaptureThreadContext(ref ContinuationContext destination, ValueTaskSourceOnCompletedFlags flags)
    {
        if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
        {
            destination.ExecutionContext = ExecutionContext.Capture();
        }

        if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
        {
            SynchronizationContext? sc = SynchronizationContext.Current;
            if (sc != null && sc.GetType() != typeof(SynchronizationContext))
            {
                destination.Scheduler = sc;
            }
            else
            {
                TaskScheduler ts = TaskScheduler.Default;
                if (ts != TaskScheduler.Default)
                {
                    destination.Scheduler = ts;
                }
            }
        }
    }

    private static void ExecuteContinuation(in ContinuationContext context, bool isAsyncExecution)
    {
        if (isAsyncExecution && context.ExecutionContext is ExecutionContext ec)
        {
            ExecutionContext.Run(ec, ecState =>
            {
                Tuple<object?, Action<object>, object> t = (Tuple<object?, Action<object>, object>)ecState;
                Invoke(t.Item1, t.Item2, t.Item3, forceAsync: false);
            }, Tuple.Create(context.Scheduler, context.Continuation, context.State));
        }
        else
        {
            Invoke(context.Scheduler, context.Continuation!, context.State!, forceAsync: true);
        }

        static void Invoke(object? scheduler, Action<object> continuation, object state, bool forceAsync)
        {
            switch (scheduler)
            {
                case SynchronizationContext sc:
                    sc.Post(s =>
                    {
                        Tuple<Action<object>, object> t = (Tuple<Action<object>, object>)s;
                        t.Item1(t.Item2);
                    }, Tuple.Create(continuation, state));
                    break;

                case TaskScheduler ts:
                    System.Threading.Tasks.Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
                    break;

                default:
                    if (forceAsync)
                    {
#if NETSTANDARD2_0
                        ThreadPool.QueueUserWorkItem(state => continuation(state), state);
#else
                        ThreadPool.QueueUserWorkItem(continuation, state, preferLocal: true);
#endif
                    }
                    else
                    {
                        continuation(state);
                    }

                    break;
            }
        }
    }
}
