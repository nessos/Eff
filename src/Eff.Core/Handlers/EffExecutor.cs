using System;
using System.Threading.Tasks;

namespace Nessos.Effects
{
    public static class EffExecutor
    {
        /// <summary>
        /// Runs supplied Eff computation using provided effect handler.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="eff">Eff computation to be run.</param>
        /// <param name="handler">Effect handler to be used in execution.</param>
        public static async Task<TResult> Run<TResult>(this Eff<TResult> eff, IEffectHandler handler)
        {   
            var result = default(TResult)!;
            var done = false;
            while (!done)
            {
                switch (eff)
                {
                    case ExceptionEff<TResult> setException:
                        await handler.Handle(setException);
                        break;
                    case ResultEff<TResult> setResult:
                        result = await handler.Handle(setResult);
                        done = true;
                        break;
                    case DelayEff<TResult> delay:
                        eff = await handler.Handle(delay);
                        break;
                    case AwaitEff<TResult> awaitEff:
                        eff = await handler.Handle(awaitEff);
                        break;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }

            return result;
        }

        /// <summary>
        /// Runs supplied Eff computation using provided effect handler.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="eff">Eff computation to be run.</param>
        /// <param name="handler">Effect handler to be used in execution.</param>
        public static Task Run(this Eff eff, IEffectHandler handler) => eff.RunCore(handler);
    }
}
