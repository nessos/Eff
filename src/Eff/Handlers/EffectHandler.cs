using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Provides an abstract effect handler implementation using regular async evaluation semantics.
    /// </summary>
    public abstract class EffectHandler : IEffectHandler
    {
        /// <summary>
        ///    Provides handling logic for abstract Effect awaiters.
        /// </summary>
        /// <typeparam name="TResult">The result type of the abstract effect.</typeparam>
        /// <param name="awaiter">The effect awaiter to be completed with a value.</param>
        /// <returns>A task waiting on the asynchronous handler.</returns>
        /// <remarks>
        ///   Typically a pattern match against subtypes of <see cref="Effect{TResult}"/>
        ///   that are recognized by the particular effect handler implementation.
        /// </remarks>
        public abstract ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter);

        /// <summary>
        ///   Provides evaluation logic for Eff state machines.
        /// </summary>
        /// <typeparam name="TResult">The result type of the eff state machine.</typeparam>
        /// <param name="stateMachine">The state machine to be evaluated.</param>
        /// <returns>A task waiting on the asynchronous evaluation.</returns>
        public virtual async ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine)
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
