using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public class EffEffect<TResult> : Effect<TResult>
    {
        public EffEffect(Eff<TResult> eff, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            Eff = eff;
        }

        public Eff<TResult> Eff { get; }

        public override Task Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }
    }
}