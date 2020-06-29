using Nessos.Effects.Utils;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Gets the position of an <see cref="EffStateMachine{TResult}" instance. />
    /// </summary>
    public enum StateMachinePosition
    {
        /// <summary>
        ///   The state machine has not been run yet.
        /// </summary>
        NotStarted = 0,

        /// <summary>
        ///   The state machine has completed with a Result value.
        /// </summary>
        Result = 1,

        /// <summary>
        ///   The state machine has completed with an Exception value.
        /// </summary>
        Exception = 2,

        /// <summary>
        ///   The state machine is suspended, pending an asynchronous operation.
        /// </summary>
        TaskAwaiter = 3,

        /// <summary>
        ///   The state machine is suspended, pending an <see cref="EffAwaiter"/> value.
        /// </summary>
        EffAwaiter = 4,
    }

    /// <summary>
    ///   Wraps Eff state machine
    /// </summary>
    public interface IEffStateMachine
    {
        /// <summary>
        ///   Gets the current position of the state machine.
        /// </summary>
        public StateMachinePosition Position { get; }

        /// <summary>
        ///   Advances the state machine to its next stage.
        /// </summary>
        public abstract void MoveNext();

        /// <summary>
        ///   Gets a heap allocated copy of the underlying compiler-generated state machine, for tracing metadata use.
        /// </summary>
        public abstract IAsyncStateMachine GetAsyncStateMachine();
    }

    /// <summary>
    ///   Represents an eff state machine awaiter.
    /// </summary>
    public abstract class EffStateMachine<TResult> : EffAwaiter<TResult>, IEffStateMachine
    {
        internal EffStateMachine()
        {

        }

        /// <summary>
        ///   Gets the current position of the state machine.
        /// </summary>
        public StateMachinePosition Position { get; protected set; } = StateMachinePosition.NotStarted;

        /// <summary>
        ///   Gets the awaiter instance, if state machine is in the <see cref="StateMachinePosition.EffAwaiter"/> state.
        /// </summary>
        /// <remarks>
        ///   Indicates that the state machine is awaiting an Eff operation, 
        ///   which must be completed by an effect handler before resuming.
        /// </remarks>
        public EffAwaiter? EffAwaiter { get; protected set; }

        /// <summary>
        ///   Gets the task instance, if state machine is in the <see cref="StateMachinePosition.TaskAwaiter"/> state.
        /// </summary>
        /// <remarks>
        ///   Indicates that the state machine is awaiting an asynchronous operation, such as a Task or ValueTask.
        ///   The returned <see cref="ValueTask" /> will complete once the underlying awaiter has also completed,
        ///   but it does not return any value or exception, since that will be captured by the underlying state machine.
        /// </remarks>
        public ValueTask? TaskAwaiter { get; protected set; }

        /// <summary>
        ///   Advances the state machine to its next stage.
        /// </summary>
        public abstract void MoveNext();

        /// <summary>
        ///   Creates a cloned copy of the state machine, in its current configuration.
        /// </summary>
        public abstract EffStateMachine<TResult> Clone();

        /// <summary>
        ///   Gets a heap allocated copy of the underlying compiler-generated state machine, for tracing metadata use.
        /// </summary>
        public abstract IAsyncStateMachine GetAsyncStateMachine();

        public override string Id => nameof(EffStateMachine<TResult>);

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);

        // Method builder helper methods

        internal void BuilderSetResult(TResult result)
        {
            SetResult(result);
            TaskAwaiter = null;
            EffAwaiter = null;
            Position = StateMachinePosition.Result;
        }

        internal void BuilderSetException(Exception e)
        {
            SetException(e);
            TaskAwaiter = null;
            EffAwaiter = null;
            Position = StateMachinePosition.Exception;
        }

        internal void BuilderSetAwaiter<TAwaiter>(ref TAwaiter awaiter)
            where TAwaiter : INotifyCompletion
        {
            if (null == (object?)default(TAwaiter) && awaiter is EffAwaiter effAwaiter)
            {
                effAwaiter.AwaitingStateMachine = this;
                TaskAwaiter = null;
                EffAwaiter = effAwaiter;
                Position = StateMachinePosition.EffAwaiter;
            }
            else
            {
                var promise = new ValueTaskPromise();
                awaiter.OnCompleted(promise.SetCompleted);
                TaskAwaiter = promise.Task;
                EffAwaiter = null;
                Position = StateMachinePosition.TaskAwaiter;
            }
        }

        internal void UnsafeBuilderSetAwaiter<TAwaiter>(ref TAwaiter awaiter)
            where TAwaiter : ICriticalNotifyCompletion
        {
            if (null == (object?)default(TAwaiter) && awaiter is EffAwaiter effAwaiter)
            {
                effAwaiter.AwaitingStateMachine = this;
                TaskAwaiter = null;
                EffAwaiter = effAwaiter;
                Position = StateMachinePosition.EffAwaiter;
            }
            else
            {
                var promise = new ValueTaskPromise();
                awaiter.UnsafeOnCompleted(promise.SetCompleted);
                TaskAwaiter = promise.Task;
                EffAwaiter = null;
                Position = StateMachinePosition.TaskAwaiter;
            }
        }
    }
}
