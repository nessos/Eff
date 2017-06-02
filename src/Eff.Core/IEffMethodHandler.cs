#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffMethodHandler
    {
        ValueTask<Eff<TResult>> Handle<TSource, TResult>(Await<TSource, TResult> await, IEffectHandler handler);
        ValueTask<Eff<TResult>> Handle<TResult>(SetResult<TResult> setResult, IEffectHandler handler);
        ValueTask<Eff<TResult>> Handle<TResult>(SetException<TResult> setException, IEffectHandler handler);
    }


}
