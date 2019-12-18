#pragma warning disable 1998
using Nessos.Eff;
using System.Threading.Tasks;

namespace Eff.Examples.DependencyInjection
{
    public class CustomEffectHandler : EffectHandler
    {
        private readonly Container _container;

        public CustomEffectHandler(Container container)
        {
            _container = container;
        }

        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DependencyEffect<TResult> _effect:
                    _effect.SetResult(_container.Get<TResult>());
                    break;
            };
        }
    }
}
