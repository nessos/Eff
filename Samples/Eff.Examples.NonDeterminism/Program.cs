using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.NonDeterminism
{
    class Program
    {

        static async Eff<(int, string)> Foo()
        {
            var x = await Effect.Choose(new[] { 1, 2, 3 });
            var y = await Effect.Choose(new[] { "one", "two", "three" });
            return (x, y);
        }

        static void Main(string[] args)
        {
            var results = Foo().Run();
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
