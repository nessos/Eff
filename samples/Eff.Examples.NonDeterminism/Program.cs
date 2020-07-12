using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.NonDeterminism
{
    class Program
    {
        static async Eff Test()
        {
            var x = await NonDetEffect.Choose(1, 2, 3);
            var y = await NonDetEffect.Choose("one", "two", "three");
            var z = await NonDetEffect.Choose(false, true);

            Console.WriteLine($"x = {x}, y = {y}, z = {z}");
        }

        static async Task Main()
        {
            await NonDetEffectHandler.Run(Test());
        }
    }
}
