using Nessos.Effects.Handlers;
using System;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Position of an Eff evaluator
    /// </summary>
    public enum EffEvaluatorPosition
    {
        NotStarted = 0,
        Result = 1,
        Exception = 2,
        Await = 3,
    }

    /// <summary>
    ///   Represents a running Eff computation
    /// </summary>
    public abstract class EffEvaluator
    {
        internal EffEvaluator() { }

        /// <summary>
        ///   Gets the current position of the evaluator
        /// </summary>
        public EffEvaluatorPosition Position { get; protected set; } = EffEvaluatorPosition.NotStarted;

        /// <summary>
        ///   Gets the exception thrown by the eff computation, if in exception state.
        /// </summary>
        public Exception? Exception { get; protected set; }

        /// <summary>
        ///   Gets the awaiter instance, if in awaited state.
        /// </summary>
        public Awaiter? Awaiter { get; protected set; }

        /// <summary>
        ///   Advances the evaluator to its next stage.
        /// </summary>
        public abstract void MoveNext();

        /// <summary>
        ///   Gets a heap allocated version of the underlying compiler-generated state machine, for metadata use.
        /// </summary>
        public abstract IAsyncStateMachine? GetStateMachine();
    }

    /// <summary>
    ///   Represents a running Eff computation
    /// </summary>
    public abstract class EffEvaluator<TResult> : EffEvaluator
    {
        /// <summary>
        ///   Gets the result of evaluator, if in completed state.
        /// </summary>
        public TResult Result { get; protected set; } = default!;

        /// <summary>
        ///   Creates a cloned copy of the evaluator, in its current configuration.
        /// </summary>
        public abstract EffEvaluator<TResult> Clone();

        // State machine helper methods

        internal void SetException(Exception e)
        {
            Position = EffEvaluatorPosition.Exception;
            Exception = e;
        }

        internal void SetResult(TResult result)
        {
            Position = EffEvaluatorPosition.Result;
            Result = result;
        }

        internal void SetAwaiter<TAwaiter>(ref TAwaiter awaiter)
            where TAwaiter : Awaiter
        {
            awaiter.SetAwaitingEvaluator(this);
            Position = EffEvaluatorPosition.Await;
            Awaiter = awaiter;
        }
    }
}
