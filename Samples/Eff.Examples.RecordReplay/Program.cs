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

        static async Eff<string> Foo()
        {
            var now = await IO.Do(_ => DateTime.UtcNow);
            var rnd = await IO.Do(ctx => ctx.Random.Next(0, 10));

            return $"{now} - {rnd}";
        }

        static void Main(string[] args)
        {
            var handler = new RecordEffectHandler(new EffCtx { Random = new Random() });
            var _ = Foo().Run(handler).Result;
            string json = handler.GetJson();
            var _handler = new ReplayEffectHandler(JsonConvert.DeserializeObject<List<Result>>(json));
            var __ = Foo().Run(_handler).Result;
        }
    }
}
