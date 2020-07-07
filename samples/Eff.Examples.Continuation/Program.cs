using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Continuation
{
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
