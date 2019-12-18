#pragma warning disable 1998
using Nessos.Eff;
using System.Threading.Tasks;

namespace Eff.Examples.CancellationToken
{
    public class CustomEffectHandler : EffectHandler
    {
        private readonly System.Threading.CancellationToken token;

        public CustomEffectHandler(System.Threading.CancellationToken token)
        {
            this.token = token;
        }

        public override async Task Handle<TResult>(TaskEffect<TResult> effect)
        {
            token.ThrowIfCancellationRequested();
            await base.Handle(effect);
        }

        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case CancellationTokenEffect _effect:
                    _effect.SetResult(token);
                    break;
            };
        }
    }
}
