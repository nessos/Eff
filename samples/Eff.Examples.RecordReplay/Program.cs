using Nessos.Effects;
using Nessos.Effects.DependencyInjection;
using Nessos.Effects.Examples.RecordReplay;

Console.WriteLine("Recording computation");
var container = new Container() { new Random() };
var recordHandler = new RecordEffectHandler(container);
await Test().Run(recordHandler);
var replayLog = recordHandler.GetReplayLog();

Console.WriteLine("Replaying computation");
var replayHandler = new ReplayEffectHandler(replayLog);
await Test().Run(replayHandler);

static async Eff Test()
{
    for (int i = 0; i < 10; i++)
    {
        int n = await IO<Random>.Do(rnd => rnd.Next(0, 100));
        Console.Write($"{n} ");
    }

    Console.WriteLine();
}