using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Gets the position 
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
        ///   The state machine is suspended, pending an <see cref="Awaiter"/> value.
        /// </summary>
        Await = 3,
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
        public abstract IAsyncStateMachine? GetAsyncStateMachine();
    }

    /// <summary>
    ///   Represents an eff state machine awaiter.
    /// </summary>
    public abstract class EffStateMachine<TResult> : Awaiter<TResult>, IEffStateMachine
    {
        /// <summary>
        ///   Gets the current position of the state machine.
        /// </summary>
        public StateMachinePosition Position { get; protected set; } = StateMachinePosition.NotStarted;

        /// <summary>
        ///   Gets the awaiter instance, if state machine is in in awaited state.
        /// </summary>
        public Awaiter? Awaiter { get; protected set; }

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
        public abstract IAsyncStateMachine? GetAsyncStateMachine();

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new TResult GetResult() => Result;

        public override string Id => nameof(EffStateMachine<TResult>);

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);

        // Method builder helper methods

        internal void BuilderSetResult(TResult result)
        {
            SetResult(result);
            Position = StateMachinePosition.Result;
        }

        internal void BuilderSetException(Exception e)
        {
            SetException(e);
            Position = StateMachinePosition.Exception;
        }

        internal void BuilderSetAwaiter(Awaiter awaiter)
        {
            awaiter.StateMachine = this;
            Awaiter = awaiter;
            Position = StateMachinePosition.Await;
        }
    }
}
