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

        public Effect(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            this.memberName = memberName;
            this.sourceFilePath = sourceFilePath;
            this.sourceLineNumber = sourceLineNumber;
        }

        public string CallerMemberName => memberName;
        public string CallerFilePath => sourceFilePath;
        public int CallerLineNumber => sourceLineNumber;

        public bool IsCompleted => hasResult || exception != null;

        public bool HasResult => hasResult;
        public Exception Exception => exception;
        public object Result => result;

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
        public abstract ValueTask<ValueTuple> Accept(IEffectHandler handler);

        public void SetException(Exception ex)
        {
            exception = ex;
        }
    }

    public static class Effect
    {

        public static TaskEffect<TResult> AsEffect<TResult>(this Task<TResult> task, 
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TaskEffect<TResult>(task, memberName, sourceFilePath, sourceLineNumber);
        }

        public static TaskEffect AsEffect(this Task task,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TaskEffect(task, memberName, sourceFilePath, sourceLineNumber);
        }

        public static EffTaskEffect<TResult> AsEffect<TResult>(this EffTask<TResult> eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffTaskEffect<TResult>(eff, memberName, sourceFilePath, sourceLineNumber);
        }
    }
    
}
