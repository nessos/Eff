using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.CancellationToken
{
    class Program
    {

        static async Eff<int> Foo()
        {
            while (true)
            {
                var token = await Effects.CancellationToken();
                Console.WriteLine($"IsCancellationRequested:{token.IsCancellationRequested}");
                await Task.Delay(1000).AsEff();
            }
        }

        static async Task Main()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var handler = new CustomEffectHandler(cts.Token);
            try
            {
                await Foo().Run(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
