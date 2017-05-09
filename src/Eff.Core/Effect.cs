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

        protected bool haveResult;
        protected TResult result;

        public Effect(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            this.memberName = memberName;
            this.sourceFilePath = sourceFilePath;
            this.sourceLineNumber = sourceLineNumber;
        }

        public string CallerMemberName => memberName;
        public string CallerFilePath => sourceFilePath;
        public int CallerLineNumber => sourceLineNumber;

        public bool IsCompleted => haveResult;
        public virtual TResult GetResult() => result;
        public IEffect<TResult> GetAwaiter() => this;

        public void SetResult(TResult result)
        {
            haveResult = true;
            this.result = result;
        }

        public abstract void OnCompleted(Action continuation);
        public abstract void UnsafeOnCompleted(Action continuation);
        public abstract ValueTask<ValueTuple> Accept(IEffectHandler handler);
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
