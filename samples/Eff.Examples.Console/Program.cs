#pragma warning disable 1998
using System;
using Nessos.Eff;
using Nessos.Eff.ImplicitAwaitables;

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
                        eff = delay.Continuation.Trigger();
                        break;
                    case Await<T> { Awaiter : Awaiter awaiter, Continuation: var kont }:
                        switch (awaiter)
                        {
                            case EffectAwaiter<Unit> { Effect: ConsolePrintEffect printEffect }:
                                System.Console.WriteLine(printEffect.Message);
                                break;
                            case EffectAwaiter<string> { Effect: ConsoleReadEffect _ } awtr :
                                string message = System.Console.ReadLine();
                                awtr.SetResult(message);
                                break;
                            default:
                                throw new NotSupportedException(awaiter.Id);
                        }
                        eff = kont.Trigger();
                        break;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }

            return result;
        }

        static void Main()
        {
            Run(Foo());
        }
    }
}
