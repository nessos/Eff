using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public abstract class EffAwaiter : ICriticalNotifyCompletion
    {
        protected bool _hasResult;
        protected Exception? _exception;
        protected object? _state;

        public abstract string Id { get; }
        public string CallerMemberName { get; set; } = "";
        public string CallerFilePath { get; set; } = "";
        public int CallerLineNumber { get; set; } = 0;

        public bool IsCompleted => _hasResult || _exception != null;
        public bool HasResult => _hasResult;
        public virtual object? Result => null;
        public Exception? Exception => _exception;
        public object? State => _state;

        public abstract Task Accept(IEffectHandler handler);

        public EffAwaiter GetAwaiter() => this;
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

    public abstract class EffAwaiter<TResult> : EffAwaiter
    {
        private TResult _result = default!;

        public new TResult GetResult()
        {
            base.GetResult();
            return _result;
        }

        public new EffAwaiter<TResult> GetAwaiter() => this;

        public EffAwaiter<TResult> ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                                  [CallerFilePath] string callerFilePath = "",
                                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            CallerMemberName = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
            return this;
        }

        public override object? Result => _result;

        public void SetResult(TResult result)
        {
            _hasResult = true;
            _result = result;
        }
    }

    public class EffEffAwaiter<TResult> : EffAwaiter<TResult>
    {
        public EffEffAwaiter(Eff<TResult> eff)
        {
            Eff = eff;
        }

        public override string Id => Eff.GetType().Name;

        public Eff<TResult> Eff { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }

    public class EffectEffAwaiter<TResult> : EffAwaiter<TResult>
    {
        public EffectEffAwaiter(Effect<TResult> effect)
        {
            Effect = effect;
        }

        public override string Id => Effect.GetType().Name;

        public Effect<TResult> Effect { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }

    public class TaskEffAwaiter<TResult> : EffAwaiter<TResult>
    {
        public TaskEffAwaiter(ValueTask<TResult> task)
        {
            Task = task;
        }

        public override string Id => "TaskAwaiter";

        public ValueTask<TResult> Task { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}
