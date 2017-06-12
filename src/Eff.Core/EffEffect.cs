using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffEffect<TResult> : Effect<TResult>
    {
        private readonly Eff<TResult> eff;

        public EffEffect(Eff<TResult> eff, string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber, captureState)
        {
            this.eff = eff;
        }

        public Eff<TResult> Eff => eff;

        public override ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }
    }
}