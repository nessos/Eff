#pragma warning disable 1998

using System;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class EffectHandler : IEffectHandler
    {

        public EffectHandler()
        {
            
        }

        public abstract Task Handle<TResult>(IEffect<TResult> effect);
       
        public virtual async Task Handle<TResult>(TaskEffect<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffEffect<TResult> effect)
        {
            var result = await effect.Eff.Run(this);
            effect.SetResult(result);
        }

        public virtual async Task<TResult> Handle<TResult>(SetResult<TResult> setResult)
        {
            return setResult.Result;
        }

        public virtual async Task Handle<TResult>(SetException<TResult> setException)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(setException.Exception).Throw();
        }

        public virtual async Task<Eff<TResult>> Handle<TResult>(Delay<TResult> delay)
        {
            return delay.Func(delay.State);
        }

        public virtual async Task<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff)
        {
            var effect = awaitEff.Effect;            
            // Execute Effect
            try
            {
                await effect.Accept(this);
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

            var eff = awaitEff.Continuation(awaitEff.State);
            return eff;
        }
    }
}
