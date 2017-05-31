using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffTaskEffect<TResult> : Effect<TResult>
    {
        private readonly EffTask<TResult> effTask;

        public EffTaskEffect(EffTask<TResult> effTask, string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber, captureState)
        {
            this.effTask = effTask;
        }

        public EffTask<TResult> EffTask => effTask;

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