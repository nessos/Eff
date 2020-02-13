using System;
using System.Threading.Tasks;

namespace Nessos.Eff.Examples.Console
{
    class Program
    {
        static async Eff Foo()
        {
            await Effects.Print("Enter your name: ");
            await Effects.Print($"Hello, { await Effects.Read()}!\n");
        }

        class ConsoleEffectHandler : EffectHandler
        {
            public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
            {
                switch (awaiter)
                {
                    case EffectAwaiter<Unit> { Effect: ConsolePrintEffect printEffect } awtr:
                        System.Console.Write(printEffect.Message);
                        awtr.SetResult(Unit.Value);
                        break;
                    case EffectAwaiter<string> { Effect: ConsoleReadEffect _ } awtr:
                        string message = System.Console.ReadLine();
                        awtr.SetResult(message);
                        break;
                    default:
                        throw new NotSupportedException(awaiter.Id);
                }

                return Task.CompletedTask;
            }
        }
        static async Task Main()
        {
            await Foo().Run(new ConsoleEffectHandler());
        }
    }
}
