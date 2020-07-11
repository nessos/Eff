using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Provides an awaiter instance for <see cref="Effect{TResult}"/> instances.
    /// </summary>
    /// <typeparam name="TResult">Result type of the awaited abstract effect.</typeparam>
    /// <remarks>
    ///   Implementers of handlers typically assign semantics to abstract effects 
    ///   by setting a result (or exception) to effect awaiter instances.
    /// </remarks>
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
        public override ValueTask Accept(IEffectHandler handler) => handler.Handle(this);
    }
}
