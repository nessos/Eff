#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.StackTrace
{
    class Program
    {
        static async Eff<int> Baz(int x)
        {
            return x + 1;
        }

        static async Eff<int> Bar(int x)
        {
            return 1 / x;
        }

        static async Eff<int> Foo(int x)
        {
            var y = await Baz(x).AsEffect();
            var z = await Bar(x).AsEffect();
            return y + z;
        }

        static async Task Main()
        {
            try
            {
                var handler = new CustomEffectHandler();
                await Foo(0).Run(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTraceLog());
            }
        }
    }
}
