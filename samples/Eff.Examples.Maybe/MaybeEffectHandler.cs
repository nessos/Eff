#pragma warning disable 1998
using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Maybe
{
    public static class MaybeEffectHandler
    {
        public static async Task<Maybe<TResult>> Run<TResult>(Eff<TResult> eff)
        {
            while (true)
            {
                switch (eff)
                {
                    case ExceptionEff<TResult> setException:
                        throw setException.Exception;
                    case ResultEff<TResult> setResult:
                        return Maybe<TResult>.Just(setResult.Result);
                    case DelayEff<TResult> delay:
                        eff = delay.StateMachine.MoveNext();
                        break;
                    case AwaitEff<TResult> awaitEff:
                        var handler = new MaybeEffectHandlerImpl<TResult>(awaitEff.StateMachine);
                        await awaitEff.Awaiter.Accept(handler);
                        return handler.Result;
                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
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
            private readonly EffStateMachine<TResult> _builder;

            public MaybeEffectHandlerImpl(EffStateMachine<TResult> builder)
            {
                _builder = builder;
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
                            await ExecuteStateMachine();
                        }
                        break;
                }
            }

            public async Task Handle<TValue>(EffAwaiter<TValue> awaiter)
            {
                var result = await MaybeEffectHandler.Run(awaiter.Eff);
                if (result.HasValue)
                {
                    awaiter.SetResult(result.Value);
                    await ExecuteStateMachine();
                }
            }

            public async Task Handle<TValue>(TaskAwaiter<TValue> awaiter)
            {
                awaiter.SetResult(await awaiter.Task);
                await ExecuteStateMachine();
            }

            public Task<TValue> Handle<TValue>(Eff<TValue> eff)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   Executes the state machine to completion using maybe semantics,
            ///   appending any results to the handler state.
            /// </summary>
            private async Task ExecuteStateMachine()
            {
                Result = await MaybeEffectHandler.Run(_builder.MoveNext());
            }
        }
    }
}
