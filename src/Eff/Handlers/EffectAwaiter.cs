using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Awaiter for abstract Effects.
    /// </summary>
    public class EffectAwaiter<TResult> : EffAwaiter<TResult>
    {
        public EffectAwaiter(Effect<TResult> effect)
        {
            Effect = effect;
        }

        /// <summary>
        ///   Gets the abstract effect that is being awaited.
        /// </summary>
        public Effect<TResult> Effect { get; }

        public override string Id => Effect.GetType().Name;
        public override Task Accept(IEffectHandler handler) => handler.Handle(this);
    }
}
