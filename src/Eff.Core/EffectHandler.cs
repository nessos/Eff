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

        public abstract Task Handle<TResult>(EffectAwaiter<TResult> effect);
       
        public virtual async Task Handle<TResult>(TaskAwaiter<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffAwaiter<TResult> effect)
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
            var awaiter = awaitEff.Awaiter;            
            // Execute Effect
            try
            {
                await awaiter.Accept(this);
                if (!awaiter.IsCompleted)
                    throw new EffException($"Effect {awaiter.Id} is not completed.");
            }
            catch (Exception ex)
            {
                awaiter.SetException(ex);
            }

            var eff = awaitEff.Continuation.Trigger();
            return eff;
        }
    }
}
