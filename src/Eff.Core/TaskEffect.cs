using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TaskEffect<TResult> : Effect<TResult>
    {
        private readonly Task<TResult> task;

        public TaskEffect(Task<TResult> task, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.task = task;
        }

        public Task<TResult> Task => task;

        public override ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
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

    public class TaskEffect : Effect<ValueTuple>
    {
        private readonly Task task;

        public TaskEffect(Task task, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.task = task;
        }

        public Task Task => task;

        public override ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
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