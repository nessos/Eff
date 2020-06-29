#pragma warning disable 1998
using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Maybe
{
    public static class MaybeEffectHandler
    {
        public static Task<Maybe<TResult>> Run<TResult>(Eff<TResult> eff) => Run(eff.GetStateMachine());

        private static async Task<Maybe<TResult>> Run<TResult>(EffStateMachine<TResult> stateMachine)
        {
            while (true)
            {
                stateMachine.MoveNext();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                        return Maybe<TResult>.Just(stateMachine.Result);
                    case StateMachinePosition.Exception:
                        throw stateMachine.Exception!;

                    case StateMachinePosition.TaskAwaiter:
                        await stateMachine.TaskAwaiter!.Value;
                        break;

                    case StateMachinePosition.EffAwaiter:
                        var awaiter = stateMachine.EffAwaiter!;
                        var handler = new MaybeEffectHandlerImpl<TResult>(stateMachine);
                        await awaiter.Accept(handler);
                        return handler.Result;

                    default:
                        throw new Exception($"Invalid state machine position {stateMachine.Position}.");
                }
            }
        }

        public static Task<Maybe<Unit>> Run(Eff eff)
        {
            return Run(Helper());

            async Eff<Unit> Helper() { await eff; return Unit.Value; }
        }

        private class MaybeEffectHandlerImpl<TResult> : IEffectHandler
        {
            private readonly EffStateMachine<TResult> _stateMachine;

            public MaybeEffectHandlerImpl(EffStateMachine<TResult> stateMachine)
            {
                _stateMachine = stateMachine;
            }

            public Maybe<TResult> Result { get; private set; } = Maybe<TResult>.Nothing;

            public async Task Handle<TValue>(EffectAwaiter<TValue> awaiter)
            {
                switch (awaiter.Effect)
                {
                    case MaybeEffect<TValue> me:
                        if (me.Result.HasValue)
                        {
                            awaiter.SetResult(me.Result.Value);
                            await ContinueStateMachine();
                        }
                        break;
                }
            }

            public async Task Handle<TValue>(EffStateMachine<TValue> stateMachine)
            {
                var result = await MaybeEffectHandler.Run(stateMachine);
                if (result.HasValue)
                {
                    await ContinueStateMachine();
                }
            }

            /// <summary>
            ///   Executes the state machine to completion using maybe semantics,
            ///   appending any results to the handler state.
            /// </summary>
            private async Task ContinueStateMachine()
            {
                Result = await MaybeEffectHandler.Run(_stateMachine);
            }
        }
    }
}
