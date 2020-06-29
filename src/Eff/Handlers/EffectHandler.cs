﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Provides a base effect handler implementation using regular async method semantics.
    /// </summary>
    public abstract class EffectHandler : IEffectHandler
    {
        public abstract Task Handle<TResult>(EffectAwaiter<TResult> awaiter);

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

                    case StateMachinePosition.TaskAwaiter:
                        await stateMachine.TaskAwaiter!.Value.ConfigureAwait(false);
                        break;

                    case StateMachinePosition.EffAwaiter:
                        var awaiter = stateMachine.EffAwaiter!;
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
