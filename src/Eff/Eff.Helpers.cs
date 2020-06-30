#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System;

namespace Nessos.Effects
{
    public abstract partial class Eff
    {
        /// <summary>
        ///   Gets an Eff instance that returns immediately.
        /// </summary>
        public static Eff CompletedEff { get; } = CompletedEffMethod();

        /// <summary>
        ///   Creates an Eff computation that returns the provided value.
        /// </summary>
        /// <param name="value">Value to be returned by the Eff computation</param>
        /// <returns>An Eff computation that returns the provided value.</returns>
        public async static Eff<TResult> FromResult<TResult>(TResult value) => value;

        /// <summary>
        ///   Creates an Eff computation that wraps the provided function body.
        /// </summary>
        /// <param name="body">Body to be wrapped by the Eff computation.</param>
        /// <returns>A delayed eff computation wrapping the function body.</returns>
        public async static Eff<TResult> FromFunc<TResult>(Func<Eff<TResult>> body) => await body().ConfigureAwait();

        /// <summary>
        ///   Creates an Eff computation that wraps the provided function body.
        /// </summary>
        /// <param name="body">Body to be wrapped by the Eff computation.</param>
        /// <returns>A delayed eff computation wrapping the function body.</returns>
        public async static Eff FromFunc(Func<Eff> body) => await body().ConfigureAwait();


        private static async Eff CompletedEffMethod()
        {

        }
    }
}
