using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TaskEffect<TResult> : Effect<TResult>
    {
        public TaskEffect(Task<TResult> task, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            Task = task;
        }

        public Task<TResult> Task { get; }

        public override Task Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }
    }
}