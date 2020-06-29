﻿using Nessos.Effects.Cancellation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.CancellationToken
{
    class Program
    {
        static async Eff Test()
        {
            while (true)
            {
                var token = await CancellationTokenEffect.Value;
                Console.WriteLine($"IsCancellationRequested:{token.IsCancellationRequested}");
                await Task.Delay(1000);
            }
        }

        static async Task Main()
        {
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
        }
    }
}
