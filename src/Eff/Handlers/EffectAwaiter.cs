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
        /// <summary>
        ///   Creates a new effect awaiter instance using the supplied abstract effect.
        /// </summary>
        /// <param name="effect">The abstract effect to be awaited.</param>
        public EffectAwaiter(Effect<TResult> effect)
        {
            Effect = effect;
        }

        /// <summary>
        ///   Gets the abstract effect that is being awaited.
        /// </summary>
        public Effect<TResult> Effect { get; }

        /// <summary>
        ///   Gets an identifier for the particular awaiter instance.
        /// </summary>
        public override string Id => Effect.GetType().Name;

        /// <summary>
        ///   Processes the awaiter using the provided effect handler.
        /// </summary>
        public override ValueTask Accept(IEffectHandler handler) => handler.Handle(this);
    }

    /// <summary>
    ///   Provides an awaiter instance for <see cref="Effect"/> instances.
    /// </summary>
    /// <remarks>
    ///   Implementers of handlers typically assign semantics to abstract effects 
    ///   by setting a result (or exception) to effect awaiter instances.
    /// </remarks>
    public class EffectAwaiter : EffectAwaiter<Unit>
    {
        /// <summary>
        ///   Creates a new effect awaiter instance using the supplied abstract effect.
        /// </summary>
        /// <param name="effect">The abstract effect to be awaited.</param>
        public EffectAwaiter(Effect effect) : base(effect) 
        {

        }

        /// <summary>
        ///   Sets a result value for the awaiter.
        /// </summary>
        public Unit SetResult() => SetResult(Unit.Value);
    }
}
