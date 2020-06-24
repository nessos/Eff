using System;
using System.Threading.Tasks;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.DependencyInjection
{

    /// <summary>
    ///   Reference effect handler implementation that populates dependency effects with container contents.
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
                    try
                    {
                        var result = await depEffect.Handle(Container).ConfigureAwait(false);
                        awaiter.SetResult(result);
                    }
                    catch (Exception e)
                    {
                        awaiter.SetException(e);
                    }

                    break;
            }
        }
    }
}
