using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffTaskEffect<TResult> : Effect<TResult>
    {
        private readonly EffTask<TResult> eff;

        public EffTaskEffect(EffTask<TResult> eff, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            this.eff = eff;
        }

        public EffTask<TResult> Eff => eff;

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