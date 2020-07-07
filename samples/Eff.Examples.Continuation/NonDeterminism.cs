#pragma warning disable 1998
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
            await eff.StartWithContinuations(async r => results.Add(r));
            return results.ToArray();
        }
    }
}
