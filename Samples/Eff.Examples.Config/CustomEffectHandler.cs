#pragma warning disable 1998
using Eff.Core;
using System.Configuration;
using System.Threading.Tasks;

namespace Eff.Examples.Config
{
    public class CustomEffectHandler : EffectHandler
    {
        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case ConfigEffect _effect:
                    var key = _effect.Key;
                    var value = ConfigurationManager.AppSettings[key];
                    _effect.SetResult(value);
                    break;
            };
        }


    }
}
