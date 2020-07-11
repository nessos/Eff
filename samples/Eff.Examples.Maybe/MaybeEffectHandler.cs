using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Maybe
{
    public static class MaybeEffectHandler
    {
        public static async Task<Maybe<TResult>> Run<TResult>(Eff<TResult> eff)
        {
            var stateMachine = eff.GetStateMachine();
            var handler = new MaybeEffectHandler<TResult>();
            await handler.Handle(stateMachine);

            return stateMachine.IsCompleted ?
                Maybe<TResult>.Just(stateMachine.GetResult()) :
                Maybe<TResult>.Nothing;
        }

        public static Task<Maybe<Unit>> Run(Eff eff)
        {
            return Run(Helper());

            async Eff<Unit> Helper() { await eff; return Unit.Value; }
        }   
    }

    public class MaybeEffectHandler<TResult> : IEffectHandler
    {
        private bool _breakExecution = false;

        public ValueTask Handle<TValue>(EffectAwaiter<TValue> awaiter)
        {
            switch (awaiter.Effect)
            {
                case MaybeEffect<TValue> me:
                    if (me.Result.HasValue)
                    {
                        awaiter.SetResult(me.Result.Value);
                    }
                    else
                    {
                        _breakExecution = true;
                    }

                    break;
            }

            return default;
        }

        public async ValueTask Handle<TValue>(EffStateMachine<TValue> stateMachine)
        {
            while (!_breakExecution)
            {
                stateMachine.MoveNext();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                    case StateMachinePosition.Exception:
                        return;

                    case StateMachinePosition.TaskAwaiter:
                        await stateMachine.TaskAwaiter!.Value;
                        break;

                    case StateMachinePosition.EffAwaiter:
                        var awaiter = stateMachine.EffAwaiter!;
                        await awaiter.Accept(this);
                        break;

                    default:
                        throw new Exception($"Invalid state machine position {stateMachine.Position}.");
                }
            }
        }

    }
}
