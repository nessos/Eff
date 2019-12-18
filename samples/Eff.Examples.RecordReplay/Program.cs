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

        static async Eff<(DateTime date, int random)> Foo()
        {
            var now = await IO.Do(_ => DateTime.UtcNow);
            var rnd = await IO.Do(ctx => ctx.Random.Next(0, 10));

            return (now, rnd);
        }

        static async Task Main()
        {
            var handler = new RecordEffectHandler(new EffCtx { Random = new Random() });
            var result = await Foo().Run(handler);
            string replayLog = handler.GetJson();
            Console.WriteLine($"Recorded: {result}");

            await Task.Delay(1000);

            var _handler = new ReplayEffectHandler(JsonConvert.DeserializeObject<List<Result>>(replayLog));
            result = await Foo().Run(_handler);
            Console.WriteLine($"Replayed: {result}");
        }
    }
}
