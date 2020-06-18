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
        /// <summary>
        ///   If set to true, will use cloned copies of Eff state machines.
        ///   Can be used to ensure thread safety of individual Eff instances.
        /// </summary>
        public virtual bool UseClonedStateMachines { get; set; } = false;

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
            while (true)
            {
                switch (eff)
                {
                    case null:
                        throw new ArgumentNullException(nameof(eff));

                    case ResultEff<TResult> setResultEff:
                        return setResultEff.Result;

                    case ExceptionEff<TResult> setExceptionEff:
                        ExceptionDispatchInfo.Capture(setExceptionEff.Exception).Throw();
                        return default!;

                    case DelayEff<TResult> delayEff:
                        var stateMachine = UseClonedStateMachines ? delayEff.StateMachine.Clone() : delayEff.StateMachine;
                        eff = stateMachine.MoveNext();
                        break;

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
