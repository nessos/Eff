#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.CancellationToken
{
    public class CustomEffectHandler : EffectHandler
    {
        private readonly System.Threading.CancellationToken token;
        public CustomEffectHandler(System.Threading.CancellationToken token) 
            : base()
        {
            this.token = token;
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            token.ThrowIfCancellationRequested();
            await base.Handle(effect);

            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case CancellationTokenEffect _effect:
                    _effect.SetResult(token);
                    break;
            };
            return ValueTuple.Create();
        }

        
    }
}
