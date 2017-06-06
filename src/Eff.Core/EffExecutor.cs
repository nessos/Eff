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
                    case Await<TResult> awaitEff:
                        var effect = awaitEff.Effect;
                        try
                        {
                            await effect.Accept(handler);
                            if (!effect.IsCompleted)
                                throw new EffException($"Effect {effect.GetType().Name} is not completed.");
                        }
                        catch (AggregateException ex)
                        {
                            effect.SetException(ex.InnerException);
                        }
                        catch (Exception ex)
                        {
                            effect.SetException(ex);
                        }

                        eff = awaitEff.Continuation();
                        break;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }

            return result;
        }
    }


}
