using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.NonDeterminism
{
    class Program
    {

        static async Eff<(int, string, bool)> Foo()
        {
            var x = await NonDetEffect.Choose(1, 2, 3);
            var y = await NonDetEffect.Choose("one", "two", "three");
            var z = await NonDetEffect.Choose(false, true);

            return (x, y, z);
        }

        static async Task Main()
        {
            var results = await NonDetEffectHandler.Run(Foo());
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
