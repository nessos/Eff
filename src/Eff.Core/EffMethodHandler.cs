#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffMethodHandler<TResult> : IEffMethodHandler<TResult>
    {
        public ValueTask<Eff<TResult>> Handle<TSource>(Await<TSource, TResult> await, IEffectHandler handler)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Eff<TResult>> Handle(SetResult<TResult> setResult, IEffectHandler handler)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Eff<TResult>> Handle(SetException<TResult> setException, IEffectHandler handler)
        {
            throw new NotImplementedException();
        }
    }


}
