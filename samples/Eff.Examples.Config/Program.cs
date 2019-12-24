using System;
using System.Threading.Tasks;
using Nessos.Eff;
using Nessos.Eff.ImplicitAwaitables;

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

        static async Task Main()
        {
            var handler = new CustomEffectHandler();
            var result = await Foo().Run(handler);
            Console.WriteLine(result);
        }
    }
}
