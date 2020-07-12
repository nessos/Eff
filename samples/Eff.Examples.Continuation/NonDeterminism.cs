#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Continuation
{
    public static class NonDeterminism
    {
        /// <summary>
        ///   Executes the current continuation nondeterministically using an array of values.
        /// </summary>
        public static Effect<TResult> Choice<TResult>(params TResult[] values)
        {
            return Effects.CallCC<TResult>(async (k, _) =>
            {
                foreach (var value in values)
                {
                    await k(value);
                }
            });
        }

        /// <summary>
        ///   Runs the provided eff computation and returns any nondeterministic results.
        /// </summary>
        public static async Task<TResult[]> Run<TResult>(Eff<TResult> eff)
        {
            Exception? exn = null;
            var results = new List<TResult>();
            await eff.StartWithContinuations(async r => results.Add(r), async e => exn = e);
            return exn == null ? results.ToArray() : throw exn;
        }

        /// <summary>
        ///   Runs the provided eff computation and returns any nondeterministic results.
        /// </summary>
        public static async Task Run(Eff eff)
        {
            Exception? exn = null;
            await eff.StartWithContinuations(async () => { }, async e => exn = e);
            if (exn != null) throw exn;
        }
    }
}
