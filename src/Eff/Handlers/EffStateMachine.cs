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
    ///   Exposes a state machine that can be used to evaluate an <see cref="Eff"/> computation.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The state machine can be advanced by calling the <see cref="MoveNext"/> method.
    ///     Depending on the state of the <see cref="Position"/> property, either of the
    ///     <see cref="Exception"/>, <see cref="EffAwaiter"/> or <see cref="TaskAwaiter"/> 
    ///     properties will be populated.
    ///   </para>
    ///   <para>
    ///     The class doubles as an <see cref="EffAwaiter"/> for <see cref="Eff"/> types. 
    ///   </para>
    /// </remarks>
    public interface IEffStateMachine
    {
        /// <summary>
        ///   Advances the state machine to its next stage.
        /// </summary>
        /// <remarks>
        ///   If current position is in either of the <see cref="StateMachinePosition.EffAwaiter"/> 
        ///   or <see cref="StateMachinePosition.TaskAwaiter"/> states, the awaiters will have to completed before
        ///   advancing the state machine state.
        /// </remarks>
        void MoveNext();

        /// <summary>
        ///   Gets the current position of the state machine.
        /// </summary>
        StateMachinePosition Position { get; }

        /// <summary>
        ///   Gets the result, if state machine is in the <see cref="StateMachinePosition.Result"/> state.
        /// </summary>
        object? Result { get; }

        /// <summary>
        ///   Gets the exception result, if state machine is in the <see cref="StateMachinePosition.Exception"/> state.
        /// </summary>
        Exception? Exception { get; }

        /// <summary>
        ///   Gets the eff awaiter instance, if state machine is in the <see cref="StateMachinePosition.EffAwaiter"/> state.
        /// </summary>
        /// <remarks>
        ///   Indicates that the state machine is awaiting an Eff operation, 
        ///   which must be completed by an effect handler before resuming.
        /// </remarks>
        EffAwaiter? EffAwaiter { get; }

        /// <summary>
        ///   Gets the task awaiter instance, if state machine is in the <see cref="StateMachinePosition.TaskAwaiter"/> state.
        /// </summary>
        /// <remarks>
        ///   Indicates that the state machine is awaiting an asynchronous operation, such as a Task or ValueTask.
        ///   The returned <see cref="ValueTask" /> will complete once the underlying awaiter has also completed,
        ///   but it does not return any value or exception, since that will be captured by the underlying state machine.
        /// </remarks>
        ValueTask? TaskAwaiter { get; }

        /// <summary>
        ///   Gets a heap allocated replica of the underlying compiler-generated state machine, for tracing metadata use.
        /// </summary>
        IAsyncStateMachine GetAsyncStateMachine();

        /// <summary>
        ///   Gets or sets the parent state machine awaiting on the current state machine.
        /// </summary>
        IEffStateMachine? AwaitingStateMachine { get; set; }

        /// <summary>
        ///   Creates a cloned copy of the state machine, in its current configuration.
        /// </summary>
        /// <remarks>
        ///   If the state machine contains an <see cref="AwaitingStateMachine"/> instance,
        ///   it will also be cloned recursively.
        /// </remarks>
        IEffStateMachine Clone();

        /// <summary>
        ///   Configures the state machine with a fresh EffAwaiter instance.
        /// </summary>
        /// <param name="awaiter">The EffAwaiter instance to use.</param>
        /// <remarks>
        ///   Used for cloning/marshalling eff continuations.
        ///   The type of the awaiter should match the type expected by the state machine.
        /// </remarks>
        void UnsafeSetAwaiter(EffAwaiter awaiter);

        /// <summary>
        ///   Processes the awaiter using the provided effect handler.
        /// </summary>
        ValueTask Accept(IEffectHandler handler);
    }

    /// <summary>
    ///   Exposes a state machine that can be used to evaluate an Eff computation.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The state machine can be advanced by calling the <see cref="MoveNext"/> method.
    ///     Depending on the state of the <see cref="Position"/> property, either of the
    ///     <see cref="Exception"/>, <see cref="EffAwaiter"/> or <see cref="TaskAwaiter"/> properties will be populated.
    ///   </para>
    ///   <para>
    ///     The class doubles as an <see cref="EffAwaiter"/> for <see cref="Eff"/> types. 
    ///   </para>
    /// </remarks>
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
        ///   Gets the eff awaiter instance, if state machine is in the <see cref="StateMachinePosition.EffAwaiter"/> state.
        /// </summary>
        /// <remarks>
        ///   Indicates that the state machine is awaiting an Eff operation, 
        ///   which must be completed by an effect handler before resuming.
        /// </remarks>
        public EffAwaiter? EffAwaiter { get; protected set; }

        /// <summary>
        ///   Gets the task awaiter instance, if state machine is in the <see cref="StateMachinePosition.TaskAwaiter"/> state.
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
        /// <remarks>
        ///   If current position is in either of the <see cref="StateMachinePosition.EffAwaiter"/> 
        ///   or <see cref="StateMachinePosition.TaskAwaiter"/> states, the awaiters will have to completed before
        ///   advancing the state machine state.
        /// </remarks>
        public abstract void MoveNext();

        /// <summary>
        ///   Creates a cloned copy of the state machine, in its current configuration.
        /// </summary>
        /// <remarks>
        ///   If the state machine contains an <see cref="AwaitingStateMachine"/> instance,
        ///   it will also be cloned recursively.
        /// </remarks>
        public abstract EffStateMachine<TResult> Clone();

        /// <summary>
        ///   Configures the state machine with a fresh EffAwaiter instance.
        /// </summary>
        /// <param name="awaiter">The EffAwaiter instance to use.</param>
        /// <remarks>
        ///   Used for cloning/marshalling eff continuations.
        ///   The type of the awaiter should match the type expected by the state machine.
        /// </remarks>
        public abstract void UnsafeSetAwaiter(EffAwaiter awaiter);

        /// <summary>
        ///   Gets a heap allocated replica of the underlying compiler-generated state machine.
        /// </summary>
        public abstract IAsyncStateMachine GetAsyncStateMachine();

        public override string Id => nameof(EffStateMachine<TResult>);

        public override ValueTask Accept(IEffectHandler handler) => handler.Handle(this);

        IEffStateMachine IEffStateMachine.Clone() => Clone();
        object? IEffStateMachine.Result => Result;

        // Method builder helper methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void BuilderSetResult(TResult result)
        {
            SetResult(result);
            TaskAwaiter = null;
            EffAwaiter = null;
            Position = StateMachinePosition.Result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void BuilderSetException(Exception e)
        {
            SetException(e);
            TaskAwaiter = null;
            EffAwaiter = null;
            Position = StateMachinePosition.Exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
