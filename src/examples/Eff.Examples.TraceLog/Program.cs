#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                sum += await Bar(i).AsEffect();
            }
            return sum;
        }

        static void Main(string[] args)
        {
            var handler = new CustomEffectHandler();
            var _ = Foo(10).Run(handler).Result;
            Console.WriteLine(handler.TraceLogs.Dump());
        }
    }
}
