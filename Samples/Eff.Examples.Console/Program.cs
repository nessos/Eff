#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.Console
{
    class Program
    {

        static async Eff<string> Foo()
        {
            string message = "Hello " + await Effect.Read();

            await Effect.Print(message);

            return message;
        }

        static T Run<T>(Eff<T> eff)
        {
            var result = default(T);
            var done = false;
            while (!done)
            {
                switch (eff)
                {
                    case SetException<T> setException:
                        throw setException.Exception;
                    case SetResult<T> setResult:
                        result = setResult.Result;
                        done = true;
                        break;
                    case Delay<T> delay:
                        eff = delay.Func(delay.State);
                        break;
                    case Await<T> awaitEff:
                        switch (awaitEff.Effect)
                        {
                            case ConsolePrintEffect printEffect:
                                System.Console.WriteLine(printEffect.Message);
                                break;
                            case ConsoleReadEffect readEffect:
                                string message = System.Console.ReadLine();
                                readEffect.SetResult(message);
                                break;
                            default:
                                throw new NotSupportedException($"{awaitEff.Effect.GetType().Name}");
                        }
                        eff = awaitEff.Continuation(awaitEff.State);
                        break;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            Run(Foo());
        }
    }
}
