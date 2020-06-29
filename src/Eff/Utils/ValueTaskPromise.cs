using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Nessos.Effects.Utils
{
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

        private volatile bool _isTaskCompleted = false;
        private ContinuationContext _firstContinuation = default;
        private ConcurrentQueue<ContinuationContext>? _additionalContinuations;
        
        private int _firstContinuationState = FCState_None;
        private const int FCState_None = 0;
        private const int FCState_WriteAcquired = 1;
        private const int FCState_WriteCompleted = 2;
        private const int FCState_ExecutionAcquired = 3;

        public ValueTask Task => new ValueTask(this, 0);

        // Signal task completion
        public void SetCompleted()
        {
            _isTaskCompleted = true;

            if (TryAcquireFirstContinuation())
            {
                ExecuteContinuation(in _firstContinuation, isAsyncExecution: true);
                _firstContinuation = default;
            }

            if (_additionalContinuations != null)
            {
                while (_additionalContinuations.TryDequeue(out var ctx))
                {
                    ExecuteContinuation(in ctx, isAsyncExecution: true);
                }
            }

            bool TryAcquireFirstContinuation()
            {
                if (_firstContinuationState > FCState_None)
                {
                    while (_firstContinuationState == FCState_WriteAcquired)
                    {
                        // Races with first OnCompleted invocation, spin until completion has been signalled.
                        Thread.SpinWait(20);
                    }

                    return Interlocked.CompareExchange(ref _firstContinuationState, FCState_ExecutionAcquired, FCState_WriteCompleted) == FCState_WriteCompleted;
                }

                return false;
            }
        }

        // Handle continuations subscribed by awaiters
        void IValueTaskSource.OnCompleted(Action<object> continuation, object state, short _, ValueTaskSourceOnCompletedFlags flags)
        {
            bool isFirstContinuation = Interlocked.CompareExchange(ref _firstContinuationState, FCState_WriteAcquired, FCState_None) == FCState_None;

            var ctx = new ContinuationContext { Continuation = continuation, State = state };
            CaptureThreadContext(ref ctx, flags);

            if (_isTaskCompleted)
            {
                // Task completed, execute the continuation immediately
                if (isFirstContinuation)
                {
                    Volatile.Write(ref _firstContinuationState, FCState_ExecutionAcquired);
                }

                ExecuteContinuation(in ctx, isAsyncExecution: false);
            }
            else
            {
                // Task not completed yet, store for future execution
                if (isFirstContinuation)
                {
                    _firstContinuation = ctx;
                    Volatile.Write(ref _firstContinuationState, FCState_WriteCompleted);
                }
                else
                {
                    var contList = GetOrCreateAdditionalContinuationQueue();
                    contList.Enqueue(ctx);
                }
            }
        }

        ValueTaskSourceStatus IValueTaskSource.GetStatus(short token) => _isTaskCompleted ? ValueTaskSourceStatus.Succeeded : ValueTaskSourceStatus.Pending;
        void IValueTaskSource.GetResult(short token) { }

        private ConcurrentQueue<ContinuationContext> GetOrCreateAdditionalContinuationQueue()
        {
            var contList = _additionalContinuations;
            if (contList == null)
            {
                contList = new ConcurrentQueue<ContinuationContext>();
                if (Interlocked.CompareExchange(ref _additionalContinuations, contList, null) is ConcurrentQueue<ContinuationContext> existing)
                {
                    contList = existing;
                }
            }

            return contList;
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
                    var t = (Tuple<object?, Action<object>, object>)ecState;
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
                            var t = (Tuple<Action<object>, object>)s;
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
}
