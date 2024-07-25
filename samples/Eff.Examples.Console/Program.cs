using System.Threading.Tasks;
using Nessos.Effects;
using Nessos.Effects.Handlers;

static async Eff Test()
{
    await ConsoleEffect.Print("Enter your name: ");
    await ConsoleEffect.Print($"Hello, {await ConsoleEffect.Read()}!\n");
}

await Test().Run(new ConsoleEffectHandler());

public class ConsolePrintEffect(string message) : Effect
{
    public string Message { get; } = message;
}

public class ConsoleReadEffect : Effect<string?>;

public static class ConsoleEffect
{
    public static ConsolePrintEffect Print(string message) => new(message);
    public static ConsoleReadEffect Read() => new();
}

/// <summary>
///   Handles console effects by calling the standard System.Console API
/// </summary>
public class ConsoleEffectHandler : EffectHandler
{
    public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
    {
        switch (awaiter)
        {
            case EffectAwaiter { Effect: ConsolePrintEffect effect } awtr:
            {
                Console.Write(effect.Message);
                awtr.SetResult();
                break;
            }
            case EffectAwaiter<string?> { Effect: ConsoleReadEffect } awtr:
            {
                string? message = Console.ReadLine();
                awtr.SetResult(message);
                break;
            }
            default:
                throw new NotSupportedException(awaiter.Effect.GetType().Name);
        }

        return default;
    }
}
