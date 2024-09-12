namespace Nessos.Effects.Examples.Config;

using Nessos.Effects.Handlers;
using System.Configuration;

public class ConfigurationManagerEffectHandler : EffectHandler
{
    public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
    {
        switch (awaiter)
        {
            case EffectAwaiter<string?> { Effect: ConfigEffect eff } awtr:
                string? value = ConfigurationManager.AppSettings[eff.Key];
                awtr.SetResult(value);
                break;
        };

        return default;
    }
}
