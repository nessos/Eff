using Nessos.Effects.Handlers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Effects
{
    /// <summary>
    ///   Represents an abstract effect, which can be awaited inside <see cref="Eff"/> methods.
    /// </summary>
    /// <typeparam name="TResult">Return type of the abstract effect.</typeparam>
    /// <remarks>
    ///   <para>
    ///     Eff users defining their own abstract effects must inherit from this class.
    ///     <see cref="Effect{TResult}"/> instances are typically declarative, 
    ///     containing no code or data guiding their evaluation semantics.
    ///   </para>
    ///   <para>
    ///     The evaluation, side-effects and result of an abstract effect is controlled by
    ///     the <see cref="IEffectHandler.Handle{TResult}(EffectAwaiter{TResult})"/> method in effect handlers.
    ///   </para>
    /// </remarks>
    public abstract class Effect<TResult>
    {
        /// <summary>
        ///   Gets an <see cref="EffectAwaiter{TResult}"/> instance containing this Effect.
        /// </summary>
        /// <returns></returns>
        public EffAwaiter<TResult> GetAwaiter() => new EffectAwaiter<TResult>(this);

        /// <summary>
        ///   Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to the method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number at the source file at which the method is called.</param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public EffAwaiter<TResult> ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new EffectAwaiter<TResult>(this)
            {
                CallerMemberName = callerMemberName,
                CallerLineNumber = callerLineNumber,
                CallerFilePath = callerFilePath
            };
        }

        /// <summary>
        ///   Executes the Effect using semantics from the provided effect handler.
        /// </summary>
        /// <param name="effectHandler">Effect handler to be used in execution.</param>
        /// <returns>A task computing the result of the Effect.</returns>
        public async Task<TResult> Run(IEffectHandler effectHandler)
        {
            var effectAwaiter = new EffectAwaiter<TResult>(this);
            await effectHandler.Handle(effectAwaiter).ConfigureAwait(false);
            return effectAwaiter.GetResult();
        }
    }

    /// <summary>
    ///   Represents an abstract effect, which can be awaited inside <see cref="Eff"/> methods.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Eff users defining their own abstract effects must inherit from this class.
    ///     <see cref="Effect"/> instances are typically declarative, 
    ///     containing no code or data guiding their evaluation semantics.
    ///   </para>
    ///   <para>
    ///     The evaluation, side-effects and result of an abstract effect is controlled by
    ///     the <see cref="IEffectHandler.Handle{TResult}(EffectAwaiter{TResult})"/> method in effect handlers.
    ///   </para>
    /// </remarks>
    public abstract class Effect : Effect<Unit>
    {

    }
}
