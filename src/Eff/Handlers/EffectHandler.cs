using System;
using System.Threading.Tasks;
using Nessos.Effects.Builders;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Provides an abstract effect handler implementation which uses regular async method semantics.
    /// </summary>
    public abstract class EffectHandler : IEffectHandler
    {
        public virtual bool CloneDelayedStateMachines { get; set; } = false;

        public abstract Task Handle<TResult>(EffectAwaiter<TResult> awaiter);
       
        public virtual async Task Handle<TResult>(TaskAwaiter<TResult> awaiter)
        {
            var result = await awaiter.Task;
            awaiter.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffAwaiter<TResult> awaiter)
        {
            var result = await EffExecutor.Execute(awaiter.Eff, this);
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

            try
            {
                await awaiter.Accept(this);
                if (!awaiter.IsCompleted)
                    throw new InvalidOperationException($"Effect {awaiter.Id} has not been completed.");
            }
            catch (Exception ex)
            {
                awaiter.SetException(ex);
            }

            return awaitEff.Continuation.MoveNext();
        }
    }
}
