using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eff.Examples.CancellationToken
{
    class Program
    {

        static async Eff<int> Foo()
        {
            while (true)
            {
                var token = await Effect.CancellationToken();
                Console.WriteLine($"IsCancellationRequested:{token.IsCancellationRequested}");
                await Task.Delay(1000).AsEffect();
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
