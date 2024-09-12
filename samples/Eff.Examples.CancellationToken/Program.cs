using Nessos.Effects;
using Nessos.Effects.Cancellation;

var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var handler = new CancellationEffectHandler(cts.Token);
try
{
    await Test().Run(handler);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

static async Eff Test()
{
    while (true)
    {
        CancellationToken token = await CancellationTokenEffect.Value;
        Console.WriteLine($"IsCancellationRequested:{token.IsCancellationRequested}");
        await Task.Delay(1000);
    }
}
