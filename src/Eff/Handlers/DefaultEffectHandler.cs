using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   An effect handler implementing default interpretation semantics which takes no action on awaited effects.
    /// </summary>
    public sealed class DefaultEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(EffectAwaiter<TResult> effect)
        {
            throw new NotSupportedException("Abstract effects not supported by this handler.");
        }
    }
}
