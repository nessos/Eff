using System.Configuration;
using System.Threading.Tasks;

namespace Nessos.Eff.Examples.Config
{
    public class CustomEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter)
            {
                case EffectAwaiter<string> { Effect: ConfigEffect { Key: string key } } awtr:
                    var value = ConfigurationManager.AppSettings[key];
                    awtr.SetResult(value);
                    break;
            };

            return Task.CompletedTask;
        }
    }
}
