using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   An effect handler providing no support for abstract effects.
    /// </summary>
    public sealed class DefaultEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            throw new NotSupportedException("Abstract effects not supported by this handler.");
        }
    }
}
