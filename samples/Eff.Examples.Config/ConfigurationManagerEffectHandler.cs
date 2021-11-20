using Nessos.Effects.Handlers;
using System.Configuration;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Config
{
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
}
