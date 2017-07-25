#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.Config
{
    public class CustomEffectHandler : EffectHandler
    {
        
        public CustomEffectHandler() 
        {
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case ConfigEffect _effect:
                    var key = _effect.Key;
                    var value = ConfigurationManager.AppSettings[key];
                    _effect.SetResult(value);
                    break;
            };
            return ValueTuple.Create();
        }


    }
}
