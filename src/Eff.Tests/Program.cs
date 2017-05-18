#pragma warning disable 1998

using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Tests
{
    class Program
    {
        static async EffTask<int> Baz(int x)
        {
            return x + 1;
        }

        static async EffTask<int> Bar(int x)
        {
            return 1 / x;
        }

        static async EffTask<int> Foo(int x)
        {
            var y = await Baz(x).AsEffect();
            var z = await Bar(x).AsEffect();
            return y + z;
        }

        static void Main(string[] args)
        {
            try
            {
                EffectExecutionContext.Handler = new TestEffectHandler();
                var _ = Foo(0).Result;
            }
            catch(AggregateException ex)
            {
                Console.WriteLine(ex.InnerException.StackTraceLog());
            }
        }
    }
}
