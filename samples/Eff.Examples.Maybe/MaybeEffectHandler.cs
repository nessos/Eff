#pragma warning disable 1998
using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Maybe
{
    public static class MaybeEffectHandler
    {
        public static Task<Maybe<TResult>> Run<TResult>(Eff<TResult> eff) => Run(eff.GetEvaluator());

        private static async Task<Maybe<TResult>> Run<TResult>(EffEvaluator<TResult> evaluator)
        {
            while (true)
            {
                evaluator.MoveNext();

                switch (evaluator.Position)
                {
                    case EffEvaluatorPosition.Result:
                        return Maybe<TResult>.Just(evaluator.Result);
                    case EffEvaluatorPosition.Exception:
                        throw evaluator.Exception!;

                    case EffEvaluatorPosition.Await:
                        var awaiter = evaluator.Awaiter!;
                        var handler = new MaybeEffectHandlerImpl<TResult>(evaluator);
                        await awaiter.Accept(handler);
                        return handler.Result;

                    default:
                        throw new Exception($"Invalid evaluator state {evaluator.Position}.");
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
            private readonly EffEvaluator<TResult> _evaluator;

            public MaybeEffectHandlerImpl(EffEvaluator<TResult> evaluator)
            {
                _evaluator = evaluator;
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

            public Task<TValue> Handle<TValue>(Eff<TValue> _)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   Executes the state machine to completion using maybe semantics,
            ///   appending any results to the handler state.
            /// </summary>
            private async Task ExecuteStateMachine()
            {
                Result = await MaybeEffectHandler.Run(_evaluator);
            }
        }
    }
}
