#pragma warning disable 1998

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class EffExecutor
    {

        public static async Task<TResult> Run<TResult>(this Eff<TResult> eff, IEffectHandler handler)
        {
            if (eff == null)
                throw new ArgumentNullException(nameof(eff));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            
            var result = default(TResult);
            var done = false;
            while (!done)
            {
                switch (eff)
                {
                    case SetException<TResult> setException:
                        await handler.Handle(setException);
                        break;
                    case SetResult<TResult> setResult:
                        result = await handler.Handle(setResult);
                        done = true;
                        break;
                    case Delay<TResult> delay:
                        eff = await handler.Handle(delay);
                        break;
                    case Await<TResult> awaitEff:
                        eff = await handler.Handle(awaitEff);
                        break;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }

            return result;
        }
    }


}
