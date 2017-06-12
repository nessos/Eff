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
                await Task.Delay(5000, await Effect.CancellationToken()).AsEffect();
            }
            return 42;
        }

        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var handler = new CustomEffectHandler(cts.Token);
            try
            {
                var _ = Foo().Run(handler).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
        }
    }
}
