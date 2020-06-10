#pragma warning disable 1998
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Maybe
{
    public static class MaybeEffectHandler
    {
        public static Maybe<TResult> Run<TResult>(Eff<TResult> eff)
        {
            switch (eff)
            {
                case ExceptionEff<TResult> setException:
                    throw setException.Exception;
                case ResultEff<TResult> setResult:
                    return Maybe<TResult>.Just(setResult.Result);
                case DelayEff<TResult> delay:
                    return Run(delay.Continuation.MoveNext());
                case AwaitEff<TResult> awaitEff:
                    var handler = new MaybeEffectHandlerImpl<TResult>(awaitEff.Continuation);
                    awaitEff.Awaiter.Accept(handler);
                    return handler.Result;
                default:
                    throw new NotSupportedException($"{eff.GetType().Name}");
            }
        }

        public static Maybe<Unit> Run(Eff eff)
        {
            return Run(Helper());

            async Eff<Unit> Helper() { await eff; return Unit.Value; }
        }

        private class MaybeEffectHandlerImpl<TResult> : EffectHandler
        {
            private readonly IEffStateMachine<TResult> _continuation;

            public MaybeEffectHandlerImpl(IEffStateMachine<TResult> continuation)
            {
                _continuation = continuation;
            }

            public Maybe<TResult> Result { get; private set; } = Maybe<TResult>.Nothing;

            public async override Task Handle<TValue>(EffectAwaiter<TValue> awaiter)
            {
                switch (awaiter.Effect)
                {
                    case MaybeEffect<TValue> me:
                        if (me.Result.HasValue)
                        {
                            awaiter.SetResult(me.Result.Value);
                            var next = _continuation.MoveNext();
                            Result = MaybeEffectHandler.Run(next);
                        }
                        break;
                }
            }

            public async override Task Handle<TValue>(EffAwaiter<TValue> awaiter)
            {
                var result = MaybeEffectHandler.Run(awaiter.Eff);
                if (result.HasValue)
                {
                    awaiter.SetResult(result.Value);
                    var next = _continuation.MoveNext();
                    Result = MaybeEffectHandler.Run(next);
                }
            }
        }
    }
}
