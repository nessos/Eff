#pragma warning disable 1998
using Nessos.Eff;
using System.Configuration;
using System.Threading.Tasks;

namespace Eff.Examples.Config
{
    public class CustomEffectHandler : EffectHandler
    {
        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter)
            {
                case EffectAwaiter<string> { Effect: ConfigEffect { Key: string key } } awtr:
                    var value = ConfigurationManager.AppSettings[key];
                    awtr.SetResult(value);
                    break;
            };
        }
    }
}
