#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffMethodHandler<TResult>
    {
        ValueTask<Eff<TResult>> Handle<TSource>(Await<TSource, TResult> await, IEffectHandler handler);
        ValueTask<Eff<TResult>> Handle(SetResult<TResult> setResult, IEffectHandler handler);
        ValueTask<Eff<TResult>> Handle(SetException<TResult> setException, IEffectHandler handler);
    }


}
