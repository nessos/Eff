using System;
using System.Threading.Tasks;
using Nessos.Effects.Builders;

namespace Nessos.Effects.Handlers
{
    public static class EffExecutor
    {
        /// <summary>
        /// Runs supplied Eff computation using provided effect handler.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="eff">Eff computation to be run.</param>
        /// <param name="handler">Effect handler to be used in execution.</param>
        public static async Task<TResult> Execute<TResult>(Eff<TResult> eff, IEffectHandler handler)
        {
            while (true)
            {
                switch (eff)
                {
                    case ExceptionEff<TResult> setException:
                        await handler.Handle(setException);
                        break;
                    case ResultEff<TResult> setResult:
                        return await handler.Handle(setResult);
                    case DelayEff<TResult> delay:
                        eff = await handler.Handle(delay);
                        break;
                    case AwaitEff<TResult> awaitEff:
                        eff = await handler.Handle(awaitEff);
                        break;
                    default:
                        throw new NotSupportedException($"Unrecognized Eff type {eff.GetType().Name}.");
                }
            }
        }
    }
}
