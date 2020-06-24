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
            try
            {
                var result = await awaiter.Task.ConfigureAwait(false);
                awaiter.SetResult(result);
            }
            catch (Exception e)
            {
                awaiter.SetException(e);
            }
        }

        public virtual async Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            while (true)
            {
                stateMachine.MoveNext();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                    case StateMachinePosition.Exception:
                        Debug.Assert(stateMachine.IsCompleted);
                        return;

                    case StateMachinePosition.Await:
                        var awaiter = stateMachine.Awaiter!;
                        try
                        {
                            await awaiter.Accept(this).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            awaiter.SetException(ex);
                        }
                        break;

                    default:
                        Debug.Fail($"Unrecognized state machine position {stateMachine.Position}.");
                        throw new Exception($"Internal error: unrecognized state machine position {stateMachine.Position}.");
                }
            }
        }
    }
}
