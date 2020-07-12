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
        public static async Eff<TResult> FromResult<TResult>(TResult value) => value;

        /// <summary>
        ///   Creates an Eff computation that wraps the provided function body.
        /// </summary>
        /// <param name="body">Body to be wrapped by the Eff computation.</param>
        /// <returns>A delayed eff computation wrapping the function body.</returns>
        public static async Eff<TResult> FromFunc<TResult>(Func<Eff<TResult>> body) => await body().ConfigureAwait();

        /// <summary>
        ///   Creates an Eff computation that wraps the provided function body.
        /// </summary>
        /// <param name="body">Body to be wrapped by the Eff computation.</param>
        /// <returns>A delayed eff computation wrapping the function body.</returns>
        public static async Eff FromFunc(Func<Eff> body) => await body().ConfigureAwait();

        /// <summary>
        ///   Creates an Eff computation that wraps the provided abstract effect.
        /// </summary>
        /// <param name="effect">Effect to be wrapped by the Eff computation.</param>
        /// <returns>A delayed eff computation wrapping the abstract effect.</returns>
        public static async Eff<TResult> FromEffect<TResult>(Effect<TResult> effect) => await effect.ConfigureAwait();

        /// <summary>
        ///   Extracts a typed Eff computation from an untyped equivalent.
        /// </summary>
        /// <param name="eff">The eff computation to wrap.</param>
        /// <returns>A computation wrapping the argument and returning a value of type <see cref="Unit"/>.</returns>
        /// <remarks>
        ///   Accelerator used in scenaria where the underlying type of the computation is not known at compile time.
        /// </remarks>
        public static Eff<Unit> FromUntypedEff(Eff eff)
        {
            if (eff is Eff<Unit> effU)
            {
                // the untyped method builder use instances of type unit,
                // so we optimize for that path.
                return effU;
            }

            return WrapUntyped(eff);

            static async Eff<Unit> WrapUntyped(Eff eff) 
            { 
                await eff.ConfigureAwait(); 
                return Unit.Value; 
            }
        }


        private static async Eff CompletedEffMethod()
        {

        }
    }
}
