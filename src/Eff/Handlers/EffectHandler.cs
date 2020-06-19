using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Nessos.Effects.Builders;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Provides a base effect handler implementation using regular async method semantics.
    /// </summary>
    public abstract class EffectHandler : IEffectHandler
    {
        public abstract Task Handle<TResult>(EffectAwaiter<TResult> awaiter);
       
        public virtual async Task Handle<TResult>(TaskAwaiter<TResult> awaiter)
        {
            var result = await awaiter.Task.ConfigureAwait(false);
            awaiter.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffAwaiter<TResult> awaiter)
        {
            var result = await Handle(awaiter.Eff).ConfigureAwait(false);
            awaiter.SetResult(result);
        }

        public virtual async Task<TResult> Handle<TResult>(Eff<TResult> eff)
        {
            if (eff is null)
            {
                throw new ArgumentNullException(nameof(eff));
            }

            while (true)
            {
                switch (eff)
                {
                    case DelayEff<TResult> delayEff:
                        eff = delayEff.CreateStateMachine().MoveNext();
                        break;

                    case ResultEff<TResult> setResultEff:
                        return setResultEff.Result;

                    case ExceptionEff<TResult> setExceptionEff:
                        ExceptionDispatchInfo.Capture(setExceptionEff.Exception).Throw();
                        return default!;

                    case AwaitEff<TResult> awaitEff:
                        var awaiter = awaitEff.Awaiter;
                        try
                        {
                            await awaiter.Accept(this).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            // clear any existing results and surface the current exception
                            if (awaiter.IsCompleted)
                            {
                                awaiter.Clear();
                            }

                            awaiter.SetException(ex);
                        }

                        eff = awaitEff.StateMachine.MoveNext();
                        break;

                    default:
                        Debug.Fail("Unrecognized Eff type.");
                        throw new Exception($"Internal error: unrecognized Eff type {eff.GetType().Name}.");
                }
            }
        }
    }
}
