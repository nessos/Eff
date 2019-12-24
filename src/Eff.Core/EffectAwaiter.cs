using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public class EffectAwaiter<TResult> : Awaiter<TResult>
    {
        public EffectAwaiter(Effect<TResult> effect, string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
            Effect = effect;
        }

        public override string Id => Effect.GetType().Name;

        public Effect<TResult> Effect { get; }

        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}