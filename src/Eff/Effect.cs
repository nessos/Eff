using Nessos.Effects.Handlers;
using System.Runtime.CompilerServices;

namespace Nessos.Effects
{
    /// <summary>
    ///   Represents an abstract effect returning no result.
    /// </summary>
    public abstract class Effect : Effect<Unit>
    {

    }

    /// <summary>
    ///   Represents an abstract effect.
    /// </summary>
    /// <typeparam name="TResult">Return type of the abstract effect.</typeparam>
    public abstract class Effect<TResult>
    {
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
    }
}
