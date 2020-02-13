using System.Threading.Tasks;

namespace Nessos.Eff.Examples.CancellationToken
{
    public class CustomEffectHandler : EffectHandler
    {
        private readonly System.Threading.CancellationToken _token;

        public CustomEffectHandler(System.Threading.CancellationToken token)
        {
            _token = token;
        }

        public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter)
            {
                case EffectAwaiter<System.Threading.CancellationToken> { Effect: CancellationTokenEffect _ } awter :
                    awter.SetResult(_token);
                    break;
            };

            return Task.CompletedTask;
        }

        public override async Task<Eff<TResult>> Handle<TResult>(AwaitEff<TResult> effect)
        {
            _token.ThrowIfCancellationRequested();
            return await base.Handle(effect);
        }
    }
}
