using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Continuation
{
    class Program
    {
        static async Eff Test()
        {
            async Eff<(int, string)> Nested()
            {
                var x = await NonDeterminism.Choice(1, 2, 3);
                var y = await NonDeterminism.Choice("a", "b", "c");
                return (x, y);
            }

            var (x, y) = await Nested();
            var z = await NonDeterminism.Choice(false, true);

            Console.WriteLine($"x = {x}, y = {y}, z = {z}");
        }

        static async Task Main()
        {
            await NonDeterminism.Run(Test());
        }
    }
}
