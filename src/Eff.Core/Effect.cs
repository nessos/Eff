using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class Effect<TResult> : IEffect<TResult>
    {
        private readonly string memberName;
        private readonly string sourceFilePath;
        private readonly int sourceLineNumber;

        protected bool hasResult;
        protected TResult result;
        protected Exception exception;
        private readonly bool captureState;
        private (string name, object value)[] parameters;
        private (string name, object value)[] localVariables;

        public Effect(string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
        {
            this.memberName = memberName;
            this.sourceFilePath = sourceFilePath;
            this.sourceLineNumber = sourceLineNumber;
            this.captureState = captureState;
        }

        public string CallerMemberName => memberName;
        public string CallerFilePath => sourceFilePath;
        public int CallerLineNumber => sourceLineNumber;

        public bool IsCompleted => hasResult || exception != null;

        public bool HasResult => hasResult;
        public Exception Exception => exception;
        public object Result => result;

        public bool CaptureState => captureState;
        public (string name, object value)[] Parameters => parameters;
        public (string name, object value)[] LocalVariables => localVariables;

        public TResult GetResult()
        {
            if (exception != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
                throw exception;
            }
            else return result;
        }
        public IEffect<TResult> GetAwaiter() => this;

        public void SetResult(TResult result)
        {
            hasResult = true;
            this.result = result;
        }

        public abstract void OnCompleted(Action continuation);
        public abstract void UnsafeOnCompleted(Action continuation);
        

        public void SetException(Exception ex)
        {
            exception = ex;
        }

        public void SetState((string name, object value)[] parameters, (string name, object value)[] localVariables)
        {
            this.parameters = parameters;
            this.localVariables = localVariables;
        }

        public Eff<TSource> Await<TSource>(Func<Eff<TSource>> continuation)
        {
            return new Await<TResult, TSource>(this, continuation);
        }

        public virtual ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }
    }

    
    
}
