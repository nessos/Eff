using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffTaskEffect<TResult> : Effect<TResult>
    {
        private readonly Eff<TResult> effTask;

        public EffTaskEffect(Eff<TResult> effTask, string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber, captureState)
        {
            this.effTask = effTask;
        }

        public Eff<TResult> EffTask => effTask;

        public override async ValueTask<ValueTuple> Accept(IEffMethodHandler handler)
        {
            //return handler.Handle(this);
            return ValueTuple.Create();
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