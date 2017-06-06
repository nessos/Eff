#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class EffExecutor
    {
        
        public static async Task<TResult> Run<TResult>(this Eff<TResult> eff, IEffectHandler handler)
        {
            var effMethodHandler = new EffMethodHandler<TResult>();
            var result = default(TResult);
            var done = false;
            while (!done)
            {
                switch (eff)
                {
                    case SetResult<TResult> setResult:
                        result = setResult.Result;
                        done = true;
                        break;
                    case Delay<TResult> delay:
                        eff = delay.Func();
                        break;
                    default:
                        eff = await eff.Handle(effMethodHandler, handler);
                        break;
                }
            }

            return result;
        }
    }


}
