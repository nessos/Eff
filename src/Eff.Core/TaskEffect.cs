using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TaskEffect<TResult> : Effect<TResult>, IAsyncEffect
    {
        private readonly Task<TResult> task;

        public TaskEffect(Task<TResult> task, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.task = task;
        }

        public Task<TResult> Task => task;

        public override void Accept(IEffectHandler handler)
        {
            throw new NotSupportedException();
        }

        public Task AcceptAsync(IEffectHandler handler)
        {
            return handler.HandleAsync(this);
        }

        public override void OnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }

        public override void UnsafeOnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }
    }
}