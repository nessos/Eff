using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffectHandler 
    {

        ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect);
        ValueTask<ValueTuple> Handle<TResult>(EffEffect<TResult> effect);

        ValueTask<TResult> Handle<TResult>(SetResult<TResult> setResult);
        ValueTask<ValueTuple> Handle<TResult>(SetException<TResult> setException);
        ValueTask<Eff<TResult>> Handle<TResult>(Delay<TResult> delay);
        ValueTask<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff);

    }

    
}
