#pragma warning disable 1998
using Eff.Core;
using Eff.Core.ImplicitAwaitables;
using System;
using System.Threading.Tasks;

namespace Eff.Examples.TraceLog
{
    class Program
    {

        static async Eff<int> Bar(int x)
        {
            return x + 1;
        }

        static async Eff<int> Foo(int n)
        {
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += await Bar(i);
            }
            return sum;
        }

        static async Task Main()
        {
            var handler = new CustomEffectHandler();
            await Foo(10).Run(handler);
            Console.WriteLine(handler.TraceLogs.Dump());
        }
    }
}
