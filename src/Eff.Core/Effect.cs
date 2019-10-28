using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class Effect<TResult> : IEffect<TResult>
    {
        protected bool _hasResult;
        protected TResult _result;
        protected Exception? _exception;
        protected object? _state;

        public Effect(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            CallerMemberName = memberName;
            CallerFilePath = sourceFilePath;
            CallerLineNumber = sourceLineNumber;
            _result = default!;
        }

        public string CallerMemberName { get; }
        public string CallerFilePath { get; }
        public int CallerLineNumber { get; }

        public bool IsCompleted => _hasResult || _exception != null;

        public bool HasResult => _hasResult;
        public Exception? Exception => _exception;
        public object Result => _result!;
        public object? State => _state;

        [return: MaybeNull]
        public TResult GetResult()
        {
            if (_exception != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(_exception).Throw();
                throw _exception;
            }
            
            return _result;
        }

        public IEffect<TResult> GetAwaiter() => this;

        public void SetResult(TResult result)
        {
            _hasResult = true;
            _result = result;
        }

        public void SetException(Exception ex)
        {
            _exception = ex;
        }

        public void SetState(object state)
        {
            _state = state;
        }

        public virtual Task Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }

        public virtual void OnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }

        public virtual void UnsafeOnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }
    }
}
