using Nessos.Effects.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.RecordReplay
{
    class Program
    {
        static async Eff<(DateTime date, int random)> Foo()
        {
            var now = await IO.Do(_ => DateTime.UtcNow);
            var rnd = await IO<Random>.Do(rnd => rnd.Next(0, 10));

            return (now, rnd);
        }

        static async Task Main()
        {
            var container = new Container() { new Random() };
            var handler = new RecordEffectHandler(container);
            var result = await Foo().Run(handler);
            var replayLog = handler.GetReplayLog();
            Console.WriteLine($"Recorded: {result}");

            await Task.Delay(1000);

            var _handler = new ReplayEffectHandler(replayLog);
            result = await Foo().Run(_handler);
            Console.WriteLine($"Replayed: {result}");
        }
    }
}
