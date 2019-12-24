using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public class EffAwaiter<TResult> : Awaiter<TResult>
    {
        public EffAwaiter(Eff<TResult> eff, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            Eff = eff;
        }

        public override string Id => Eff.GetType().Name;

        public Eff<TResult> Eff { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}