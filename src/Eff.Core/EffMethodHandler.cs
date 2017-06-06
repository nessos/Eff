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
        public async ValueTask<Eff<TResult>> Handle<TSource>(Await<TSource, TResult> awaitEff, IEffectHandler handler)
        {
            var result = default(TSource);
            await awaitEff.Effect.Accept(handler);
            return awaitEff.Success(result);
        }

        public async ValueTask<Eff<TResult>> Handle(SetResult<TResult> setResult, IEffectHandler handler)
        {
            return setResult;
        }

        public async ValueTask<Eff<TResult>> Handle(SetException<TResult> setException, IEffectHandler handler)
        {
            return setException;
        }

        public async ValueTask<Eff<TResult>> Handle(Delay<TResult> delay, IEffectHandler handler)
        {
            return delay;
        }
    }


}
