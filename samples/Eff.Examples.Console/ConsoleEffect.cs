namespace Nessos.Eff.Examples.Console
{
    public class ConsolePrintEffect : Effect
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

    public static class Effects
    {
        public static ConsolePrintEffect Print(string message) => new ConsolePrintEffect(message);

        public static ConsoleReadEffect Read() => new ConsoleReadEffect();
    }
}