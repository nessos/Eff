using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Console
{
    /// <summary>
    ///   Handles console effects by calling the standard System.Console API
    /// </summary>
    public class ConsoleEffectHandler : EffectHandler
    {
        public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter)
            {
                case EffectAwaiter { Effect: ConsolePrintEffect printEffect } awtr:
                    System.Console.Write(printEffect.Message);
                    awtr.SetResult();
                    break;
                case EffectAwaiter<string> { Effect: ConsoleReadEffect _ } awtr:
                    string message = System.Console.ReadLine();
                    awtr.SetResult(message);
                    break;
                default:
                    throw new NotSupportedException(awaiter.Effect.GetType().Name);
            }

            return default;
        }
    }
}
