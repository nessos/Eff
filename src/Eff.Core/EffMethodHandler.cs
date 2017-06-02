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
            switch (awaitEff.Effect)
            {
                case TaskEffect<TSource> taskEffect:
                    result = await handler.Handle(taskEffect);
                    break;
                case TaskEffect taskEffect:
                    await handler.Handle(taskEffect);
                    break;
                case FuncEffect<TSource> funcEffect:
                    result = await handler.Handle(funcEffect);
                    break;
                case ActionEffect actionEffect:
                    await handler.Handle(actionEffect);
                    break;
                default:
                    result = await handler.Handle(awaitEff.Effect);
                    break;
            }
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
    }


}
