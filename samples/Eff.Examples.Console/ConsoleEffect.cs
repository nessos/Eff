using Nessos.Eff;
using System;
using System.Runtime.CompilerServices;

namespace Eff.Examples.Console
{
    public class ConsolePrintEffect : Effect<Unit>
    {
        public ConsolePrintEffect(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class ConsoleReadEffect : Effect<string>
    {

    }

    public static class Effect
    {
        public static ConsolePrintEffect Print(string message) => new ConsolePrintEffect(message);

        public static ConsoleReadEffect Read() => new ConsoleReadEffect();
    }
}