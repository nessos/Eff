using System;

namespace Nessos.Effects.Examples.NonDeterminism
{
    class Program
    {

        static async Eff<(int, string)> Foo()
        {
            var x = await NonDetEffect.Choose(1, 2, 3);
            var y = await NonDetEffect.Choose("one", "two", "three");
            return (x, y);
        }

        static void Main()
        {
            var results = NonDetEffectHandler.Run(Foo());
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
