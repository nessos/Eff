using Eff.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    class Program
    {

        static async Eff<string> Foo<T>()
            where T : struct, IDateTimeNowEffect, IRandomEffect
        {
            var now = await default(T).DateTimeNow();
            var rnd = await default(T).Random();

            return $"{now} - {rnd}";
        }

        static void Main(string[] args)
        {
            var handler = new RecordEffectHandler();
            var _ = Foo<EffectExample>().Run(handler).Result;
            string json = handler.GetJson();
            var _handler = new ReplayEffectHandler(JsonConvert.DeserializeObject<List<Result>>(json));
            var __ = Foo<EffectExample>().Run(_handler).Result;
        }
    }
}
