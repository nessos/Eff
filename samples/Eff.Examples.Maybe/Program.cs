using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Maybe
{
    class Program
    {
        static async Eff<int> Divide(int m, int n)
        {
            return (n == 0) ? await MaybeEffect.Nothing<int>() : await MaybeEffect.Just<int>(m / n);
        }

        static async Eff DivideAndReportToConsole(int m, int n)
        {
            Console.Write($"Calculating {m} / {n}: ");
            var result = await Divide(m, n);
            Console.WriteLine($"Got {result}!");
        }

        static async Eff Test()
        {
            for (int i = 0; i < 100; i++)
            {
                await DivideAndReportToConsole(23, 5 - i);
            }
        }

        static async Task Main()
        {
            await MaybeEffectHandler.Run(Test());
        }
    }
}
