#pragma warning disable 1998
using System;
using System.Threading.Tasks;

namespace Nessos.Eff.Examples.StackTrace
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
            var y = await Baz(x);
            var z = await Bar(x);
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
