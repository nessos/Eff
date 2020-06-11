using System;
using System.Threading.Tasks;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.DependencyInjection
{

    /// <summary>
    ///   Reference effect handler implementation that passes container instances to dependency effects.
    /// </summary>
    public class DependencyEffectHandler : EffectHandler
    {
        protected IContainer Container { get; }

        public DependencyEffectHandler(IContainer container)
        {
            Container = container;
        }

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter.Effect)
            {
                case DependencyEffect<TResult> depEffect:
                    var result = await depEffect.Handle(Container);
                    awaiter.SetResult(result);

                    break;
            }
        }
    }
}
