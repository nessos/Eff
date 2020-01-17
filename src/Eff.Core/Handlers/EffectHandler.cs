using System;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public abstract class EffectHandler : IEffectHandler
    {
        public virtual bool CloneDelayedStateMachines { get; set; } = false;

        public abstract Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter);
       
        public virtual async Task Handle<TResult>(TaskEffAwaiter<TResult> awaiter)
        {
            var result = await awaiter.Task;
            awaiter.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffEffAwaiter<TResult> awaiter)
        {
            var result = await awaiter.Eff.Run(this);
            awaiter.SetResult(result);
        }

        public virtual Task<TResult> Handle<TResult>(ResultEff<TResult> setResultEff) => Task.FromResult(setResultEff.Result);

        public virtual Task Handle<TResult>(ExceptionEff<TResult> setExceptionEff)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(setExceptionEff.Exception).Throw();
            return default!;
        }

        public virtual Task<Eff<TResult>> Handle<TResult>(DelayEff<TResult> delayEff)
        {
            return Task.FromResult(delayEff.Continuation.MoveNext(useClonedStateMachine: CloneDelayedStateMachines));
        }

        public virtual async Task<Eff<TResult>> Handle<TResult>(AwaitEff<TResult> awaitEff)
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

            var eff = awaitEff.Continuation.MoveNext();
            return eff;
        }
    }
}
