using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   An effect handler providing no support for abstract effects.
    /// </summary>
    /// <remarks>
    ///   Throws a <see cref="NotSupportedException"/> when attempting to evaluating
    ///   <see cref="Eff"/> computations awaiting on abstract <see cref="Effect"/> instances.
    /// </remarks>
    public sealed class DefaultEffectHandler : EffectHandler
    {
        /// <summary>
        ///   An effect handler that always throws.
        /// </summary>
        /// <exception cref="NotSupportedException" />
        public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            throw new NotSupportedException("Abstract effects not supported by this handler.");
        }
    }
}
