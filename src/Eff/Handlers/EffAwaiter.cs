using Nessos.Effects.Builders;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Awaiter class for Eff computations.
    /// </summary>
    public abstract class Awaiter : ICriticalNotifyCompletion
    {
        protected bool _hasResult;
        protected Exception? _exception;
        protected EffEvaluator? _evaluator;

        internal Awaiter() { }

        /// <summary>
        ///   Awaiter identifier for debugging purposes.
        /// </summary>
        public abstract string Id { get; }

        public string CallerMemberName { get; set; } = "";
        public string CallerFilePath { get; set; } = "";
        public int CallerLineNumber { get; set; } = 0;

        /// <summary>
        ///   Returns true if the awaiter has been completed with a result value.
        /// </summary>
        public bool HasResult => _hasResult;

        /// <summary>
        ///   Returns true if the awaiter has been completed with an exception value.
        /// </summary>
        public bool HasException => !(_exception is null);

        /// <summary>
        ///   Returns true if the awaiter has been completed with either a result or an exception.
        /// </summary>
        public bool IsCompleted => HasResult || HasException;

        /// <summary>
        ///   Gets either the result value or throws the exception that have been stored in the awaiter.
        /// </summary>
        public object? Result => GetResultUntyped();

        /// <summary>
        ///   Gets the exception result for the awaiter.
        /// </summary>
        public Exception? Exception => _exception;

        /// <summary>
        ///   Gets the evaluator instance associated with the awaiter.
        /// </summary>
        public EffEvaluator? AwaitingEvaluator => _evaluator;

        /// <summary>
        ///   Sets a result value for the awaiter.
        /// </summary>
        public abstract void SetResult(object? value);

        /// <summary>
        ///   Sets an exception value for the awaiter.
        /// </summary>
        public void SetException(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException();
            }

            if (IsCompleted)
            {
                throw new InvalidOperationException("EffAwaiter has already been completed.");
            }

            _exception = exception;
        }


        /// <summary>
        ///   Sets the evaluator instance associated with the awaiter.
        /// </summary>
        internal void SetAwaitingEvaluator(EffEvaluator evaluator) => _evaluator = evaluator;

        /// <summary>
        ///   Processes the awaiter using the provided effect handler.
        /// </summary>
        public abstract Task Accept(IEffectHandler handler);

        /// <summary>
        ///   Clears any results from the awaiter instance.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///   Configures the EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public Awaiter ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        public Awaiter GetAwaiter() => this;

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void GetResult()
        {
            if (!(_exception is null))
            {
                ExceptionDispatchInfo.Capture(_exception).Throw();
                throw _exception;
            }

            if (!_hasResult)
            {
                throw new InvalidOperationException("EffAwaiter has not been completed.");
            }
        }

        // workaround for no covariant return types
        protected abstract object? GetResultUntyped();

        void INotifyCompletion.OnCompleted(Action continuation) => throw new NotSupportedException("Eff awaitables should only be awaited in Eff methods.");
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => throw new NotSupportedException("Eff awaitables should only be awaited in Eff methods.");
    }

    /// <summary>
    ///   Awaiter class for Eff computations.
    /// </summary>
    public abstract class Awaiter<TResult> : Awaiter
    {
        [AllowNull]
        private TResult _result = default;

        /// <summary>
        ///   Gets either the result value or throws the exception that have been stored in the awaiter.
        /// </summary>
        public new TResult Result
        {
            get
            {
                if (!(_exception is null))
                {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                    throw _exception;
                }

                
                if (!_hasResult)
                {
                    throw new InvalidOperationException($"Awaiter of type {Id} has not been completed.");
                }

                return _result;
            }
        }

        /// <summary>
        ///   Sets a result value for the awaiter.
        /// </summary>
        public void SetResult(TResult value)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("EffAwaiter has already been completed.");
            }

            _result = value;
            _hasResult = true;
        }

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new TResult GetResult() => Result;

        /// <summary>
        ///   For use by EffMethodBuilder
        /// </summary>
        public new Awaiter<TResult> GetAwaiter() => this;

        /// <summary>
        ///   Configures the EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public new Awaiter<TResult> ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }

        public override void SetResult(object? value)
        {
            SetResult((TResult)value!);
        }

        protected override object? GetResultUntyped() => Result;

        public override void Clear()
        {
            _result = default;
            _hasResult = false;
            _exception = null;
        }
    }

    /// <summary>
    ///   Awaiter for nested Eff computations.
    /// </summary>
    public class EffAwaiter<TResult> : Awaiter<TResult>
    {
        public EffAwaiter(Eff<TResult> eff)
        {
            Eff = eff;
        }

        public Eff<TResult> Eff { get; }

        public override string Id => Eff.GetType().Name;
        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }

    /// <summary>
    ///   Awaiter for abstract Effects.
    /// </summary>
    public class EffectAwaiter<TResult> : Awaiter<TResult>
    {
        public EffectAwaiter(Effect<TResult> effect)
        {
            Effect = effect;
        }

        public Effect<TResult> Effect { get; }

        public override string Id => Effect.GetType().Name;
        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }

    /// <summary>
    ///   Awaiter adapter for TPL tasks.
    /// </summary>
    public class TaskAwaiter<TResult> : Awaiter<TResult>
    {
        public TaskAwaiter(ValueTask<TResult> task)
        {
            Task = task;
        }

        public ValueTask<TResult> Task { get; }

        public override string Id => nameof(TaskAwaiter);
        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}
