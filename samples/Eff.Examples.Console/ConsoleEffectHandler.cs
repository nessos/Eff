using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Console
{
    // returns Unit instead of void
    public static class ConsoleF{
        public static Unit write(string message) {
             System.Console.Write(message);
             return default;
        }
    }

    /// <summary>
    ///   Handles console effects by calling the standard System.Console API
    /// </summary>
    public class ConsoleEffectHandler : EffectHandler
    {
        
        public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter) =>
           awaiter switch
           {               
               EffectAwaiter { Effect: ConsolePrintEffect printEffect } write => 
                    write.SetResult(ConsoleF.write(printEffect.Message)),
               EffectAwaiter<string> { Effect: ConsoleReadEffect _ } read => 
                    read.SetResult(System.Console.ReadLine()),
               //if new case will arrive, will get `System.Runtime.CompilerServices.SwitchExpressionException: Non-exhaustive switch expression failed to match its input.`
           };
    }
}
