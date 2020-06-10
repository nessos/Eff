using System.Threading;

namespace Nessos.Effects.Cancellation
{
    public static class CancellationTokenEffect
    {
        /// <summary>
        ///   Defines the cancellation token abstract effect.
        /// </summary>
        public static Effect<CancellationToken> Value { get; } = new Effect();

        internal class Effect : Effect<CancellationToken>
        {

        }
    }
}
