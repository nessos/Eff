using Nessos.Effects.Handlers;
using System;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Eff instance representing a delayed computation.
    /// </summary>
    public sealed class DelayEff<TResult> : Eff<TResult>
    {
        internal DelayEff(EffStateMachine<TResult> stateMachine)
        {
            StateMachine = stateMachine;
        }

        public EffStateMachine<TResult> StateMachine { get; }
    }

    /// <summary>
    ///   Eff instance representing an await-ed computation
    /// </summary>
    public sealed class AwaitEff<TResult> : Eff<TResult>
    {
        internal AwaitEff(Awaiter awaiter, EffStateMachine<TResult> stateMachine)
        {
            Awaiter = awaiter;
            StateMachine = stateMachine;
        }

        /// <summary>
        ///   The eff awaiter that the computation awaits.
        /// </summary>
        public Awaiter Awaiter { get; }

        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public EffStateMachine<TResult> StateMachine { get; }
    }

    /// <summary>
    ///   Eff instance representing a completed computation.
    /// </summary>
    public sealed class ResultEff<TResult> : Eff<TResult>
    {
        internal ResultEff(TResult result, object state)
        {
            Result = result;
            State = state;
        }

        /// <summary>
        ///   Materialized result of the computation
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public object State { get; }
    }

    /// <summary>
    ///   Eff instance representing an exceptional computation.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class ExceptionEff<TResult> : Eff<TResult>
    {
        internal ExceptionEff(Exception exception, object state)
        {
            Exception = exception;
            State = state;
        }

        /// <summary>
        ///   Materialized exception of the computation
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///   The current state object of the machine.
        /// </summary>
        public object State { get; }
    }
}
