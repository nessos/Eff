using Nessos.Effects.Handlers;
using System;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Eff instance representing a delayed computation.
    /// </summary>
    public abstract class DelayEff<TResult> : Eff<TResult>
    {
        internal DelayEff() { }

        /// <summary>
        ///   Creates a state machine instance that can be evaluated.
        /// </summary>
        /// <returns></returns>
        public abstract EffStateMachine<TResult> CreateStateMachine();
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
        internal ResultEff(TResult result, EffStateMachine<TResult> stateMachine)
        {
            Result = result;
            StateMachine = stateMachine;
        }

        /// <summary>
        ///   Materialized result of the computation
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public EffStateMachine<TResult> StateMachine { get; }
    }

    /// <summary>
    ///   Eff instance representing an exceptional computation.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class ExceptionEff<TResult> : Eff<TResult>
    {
        internal ExceptionEff(Exception exception, EffStateMachine<TResult> stateMachine)
        {
            Exception = exception;
            StateMachine = stateMachine;
        }

        /// <summary>
        ///   Materialized exception of the computation
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///   The current state object of the machine.
        /// </summary>
        public EffStateMachine<TResult> StateMachine { get; }
    }
}
