using System.Threading.Tasks;

namespace Nessos.Effects
{
    /// <summary>
    /// An effect handler implementing default interpretation semantics which takes no action on awaited effects.
    /// </summary>
    public class DefaultEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(EffectAwaiter<TResult> effect) => Task.CompletedTask;
    }
}
