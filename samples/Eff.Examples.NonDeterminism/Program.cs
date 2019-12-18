using Nessos.Eff;
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
            var x = await Effect.Choose(1, 2, 3);
            var y = await Effect.Choose("one", "two", "three");
            return (x, y);
        }

        static void Main()
        {
            var results = Foo().Run();
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
