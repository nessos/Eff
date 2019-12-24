using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public class TaskAwaiter<TResult> : Awaiter<TResult>
    {
        public TaskAwaiter(Task<TResult> task, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            Task = task;
        }

        public override string Id => "TaskAwaiter";

        public Task<TResult> Task { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}