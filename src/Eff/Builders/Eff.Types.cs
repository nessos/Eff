using Nessos.Effects.Handlers;
using System;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Eff instance representing a delayed computation.
    /// </summary>
    public sealed class DelayEff<TResult> : Eff<TResult>
    {
        internal DelayEff(IEffStateMachine<TResult> continuation)
        {
            Continuation = continuation;
        }

        public IEffStateMachine<TResult> Continuation { get; }
    }

    /// <summary>
    ///   Eff instance representing an await-ed computation
    /// </summary>
    public sealed class AwaitEff<TResult> : Eff<TResult>
    {
        internal AwaitEff(EffAwaiterBase awaiter, IEffStateMachine<TResult> continuation)
        {
            Awaiter = awaiter;
            Continuation = continuation;
        }

        public EffAwaiterBase Awaiter { get; }

        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public IEffStateMachine<TResult> Continuation { get; }
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
