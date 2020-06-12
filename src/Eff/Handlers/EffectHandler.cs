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
        ///   Determines whether the handler should clone Eff state machines before executing them.
        ///   Should only be used in handler scenaria where the state machine will be executed more than once,
        ///   e.g. in nondeterministic handlers. Defaults to false.
        /// </summary>
        public virtual bool CloneDelayedStateMachines { get; set; } = false;

        public abstract Task Handle<TResult>(EffectAwaiter<TResult> awaiter);
       
        public virtual async Task Handle<TResult>(TaskAwaiter<TResult> awaiter)
        {
            var result = await awaiter.Task;
            awaiter.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffAwaiter<TResult> awaiter)
        {
            var result = await Handle(awaiter.Eff);
            awaiter.SetResult(result);
        }

        public virtual async Task<TResult> Handle<TResult>(Eff<TResult> eff)
        {
            while (true)
            {
                switch (eff)
                {
                    case ResultEff<TResult> setResultEff:
                        return setResultEff.Result;

                    case ExceptionEff<TResult> setExceptionEff:
                        ExceptionDispatchInfo.Capture(setExceptionEff.Exception).Throw();
                        return default!;

                    case DelayEff<TResult> delayEff:
                        eff = delayEff.Continuation.MoveNext(useClonedStateMachine: CloneDelayedStateMachines);
                        break;

                    case AwaitEff<TResult> awaitEff:
                        var awaiter = awaitEff.Awaiter;
                        try
                        {
                            await awaiter.Accept(this);
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

                        eff = awaitEff.Continuation.MoveNext();
                        break;

                    default:
                        Debug.Fail("Unrecognized Eff type.");
                        throw new Exception($"Internal error: unrecognized Eff type {eff.GetType().Name}.");
                }
            }
        }
    }
}
