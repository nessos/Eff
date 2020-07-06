#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Continuation
{
    public static class NonDeterminism
    {
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

        public static async Task<TResult[]> Run<TResult>(Eff<TResult> eff)
        {
            var results = new List<TResult>();
            await eff.RunWithContinuations(async r => results.Add(r));
            return results.ToArray();
        }
    }

    class Program
    {
        static async Eff<(int, string, bool)> Test()
        {
            async Eff<(int, string)> Nested()
            {
                var x = await NonDeterminism.Choice(1, 2, 3);
                var y = await NonDeterminism.Choice("a", "b", "c");
                return (x, y);
            }

            var (x, y) = await Nested();
            var z = await NonDeterminism.Choice(false, true);

            return (x, y, z);
        }

        static async Task Main()
        {
            var results = await NonDeterminism.Run(Test());

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
