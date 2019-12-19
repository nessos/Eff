using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public static class EffExecutor
    {

        public static async Task<TResult> Run<TResult>(this Eff<TResult> eff, IEffectHandler handler)
        {   
            var result = default(TResult)!;
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

        public static Task Run(this Eff eff, IEffectHandler handler) => eff.Ignore().Run(handler);
    }
}
