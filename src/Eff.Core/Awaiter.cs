using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public abstract class Awaiter : ICriticalNotifyCompletion
    {
        protected bool _hasResult;
        protected Exception? _exception;
        protected object? _state;

        public Awaiter(string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            CallerMemberName = memberName;
            CallerFilePath = sourceFilePath;
            CallerLineNumber = sourceLineNumber;
        }

        public abstract string Id { get; }
        public string CallerMemberName { get; }
        public string CallerFilePath { get; }
        public int CallerLineNumber { get; }

        public bool IsCompleted => _hasResult || _exception != null;
        public bool HasResult => _hasResult;
        public virtual object? Result => null;
        public Exception? Exception => _exception;
        public object? State => _state;

        public abstract Task Accept(IEffectHandler handler);

        public Awaiter GetAwaiter() => this;
        internal void SetException(Exception ex) => _exception = ex;
        internal void SetState(object state) => _state = state;
        void INotifyCompletion.OnCompleted(Action continuation) => throw new NotSupportedException();
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => throw new NotSupportedException();
    }

    public abstract class Awaiter<TResult> : Awaiter
    {
        private TResult _result;

        public Awaiter(string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            _result = default!;
        }

        public TResult GetResult()
        {
            // TODO: make compatible with Task.Result semantics

            if (!(_exception is null))
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(_exception).Throw();
                throw _exception;
            }

            return _result;
        }

        public new Awaiter<TResult> GetAwaiter() => this;

        public override object? Result => _result;

        public void SetResult(TResult result)
        {
            _hasResult = true;
            _result = result;
        }
    }
}
