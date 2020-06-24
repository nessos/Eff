using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Config
{
    class Program
    {
        public static async Eff<string> Test()
        {
            var value1 = await ConfigEffect.Get("Setting1");
            var value2 = await ConfigEffect.Get("Setting2");

            return $"{value1} - {value2}";
        }

        static async Task Main()
        {
            var handler = new ConfigurationManagerEffectHandler();
            var result = await Test().Run(handler);
            Console.WriteLine(result);
        }
    }
}
