using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.Config
{
    class Program
    {

        public static async Eff<string> Foo()
        {
            var google = await Effect.Config("google");
            var microsoft = await Effect.Config("microsoft");

            return $"{google} - {microsoft}";
        }

        static void Main(string[] args)
        {
            var handler = new CustomEffectHandler();
            var result = Foo().Run(handler).Result;
            Console.WriteLine(result);
        }
    }
}
