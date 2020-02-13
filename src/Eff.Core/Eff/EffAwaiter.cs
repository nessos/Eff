using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    /// <summary>
    /// Awaiter class for Eff computations.
    /// </summary>
    public abstract class EffAwaiterBase : ICriticalNotifyCompletion
    {
        protected bool _hasResult;
        protected Exception? _exception;
        protected object? _state;

        internal EffAwaiterBase() { }

        /// <summary>
        /// Awaiter identifier for debugging purposes.
        /// </summary>
        public abstract string Id { get; }

        public string CallerMemberName { get; set; } = "";
        public string CallerFilePath { get; set; } = "";
        public int CallerLineNumber { get; set; } = 0;

        public bool IsCompleted => _hasResult || _exception != null;
        public bool HasResult => _hasResult;
        public abstract object? Result { get; }
        public Exception? Exception => _exception;
        public object? State => _state;

        public abstract Task Accept(IEffectHandler handler);

        /// <summary>
        /// Configures the EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public EffAwaiterBase ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }

        /// <summary>
        /// For use by EffMethodBuilder
        /// </summary>
        public EffAwaiterBase GetAwaiter() => this;
        /// <summary>
        /// For use by EffMethodBuilder
        /// </summary>
        public void GetResult()
        {
            if (!(_exception is null))
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(_exception).Throw();
                throw _exception;
            }

            if (!_hasResult)
            {
                throw new InvalidOperationException($"Effect awaiter has not been completed yet");
            }
        }

        internal void SetException(Exception ex) => _exception = ex;
        internal void SetState(object state) => _state = state;
        void INotifyCompletion.OnCompleted(Action continuation) => throw new NotSupportedException("EffAwaiter objects should only be awaited by EffMethodBuilder");
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => throw new NotSupportedException("EffAwaiter objects should only be awaited by EffMethodBuilder");
    }

    /// <summary>
    /// Awaiter class for Eff computations.
    /// </summary>
    public abstract class EffAwaiterBase<TResult> : EffAwaiterBase
    {
        private TResult _result = default!;

        internal EffAwaiterBase() { }

        /// <summary>
        /// For use by EffMethodBuilder
        /// </summary>
        public new TResult GetResult()
        {
            base.GetResult();
            return _result;
        }

        /// <summary>
        /// For use by EffMethodBuilder
        /// </summary>
        public new EffAwaiterBase<TResult> GetAwaiter() => this;

        /// <summary>
        /// Configures the EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public new EffAwaiterBase<TResult> ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                                      [CallerFilePath] string callerFilePath = "",
                                                      [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }

        public override object? Result => GetResult();

        public void SetResult(TResult result)
        {
            _hasResult = true;
            _result = result;
        }
    }

    /// <summary>
    /// Awaiter for nested Eff computations.
    /// </summary>
    public class EffAwaiter<TResult> : EffAwaiterBase<TResult>
    {
        internal EffAwaiter(Eff<TResult> eff)
        {
            Eff = eff;
        }

        public override string Id => Eff.GetType().Name;

        public Eff<TResult> Eff { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }

    /// <summary>
    /// Awaiter for abstract Effects.
    /// </summary>
    public class EffectAwaiter<TResult> : EffAwaiterBase<TResult>
    {
        public EffectAwaiter(Effect<TResult> effect)
        {
            Effect = effect;
        }

        public override string Id => Effect.GetType().Name;

        public Effect<TResult> Effect { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }

    /// <summary>
    /// Awaiter adapter for TPL tasks.
    /// </summary>
    public class TaskAwaiter<TResult> : EffAwaiterBase<TResult>
    {
        public TaskAwaiter(ValueTask<TResult> task)
        {
            Task = task;
        }

        public override string Id => "TaskAwaiter";

        public ValueTask<TResult> Task { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}
