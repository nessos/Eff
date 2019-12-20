using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public abstract class EffectHandler : IEffectHandler
    {
        public EffectHandler()
        {
            
        }

        public virtual bool CloneDelayedStateMachines { get; set; } = false;

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

        public virtual Task<TResult> Handle<TResult>(SetResult<TResult> setResult) => Task.FromResult(setResult.Result);

        public virtual Task Handle<TResult>(SetException<TResult> setException)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(setException.Exception).Throw();
            return default!;
        }

        public virtual Task<Eff<TResult>> Handle<TResult>(Delay<TResult> delay)
        {
            return Task.FromResult(delay.Continuation.Trigger(useClonedStateMachine: CloneDelayedStateMachines));
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

            var eff = awaitEff.Continuation.Trigger();
            return eff;
        }
    }
}
