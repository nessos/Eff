#pragma warning disable 1998
using Nessos.Effects;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.DependencyInjection
{
    public class CustomEffectHandler : EffectHandler
    {
        private readonly Container _container;

        public CustomEffectHandler(Container container)
        {
            _container = container;
        }

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter.Effect)
            {
                case DependencyEffect<TResult> _:
                    awaiter.SetResult(_container.Get<TResult>());
                    break;
            };
        }
    }
}
