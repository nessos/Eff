#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Examples.NonDeterminism
{
    public static class NonDetEffectHandler
    {
        public static List<TResult> Run<TResult>(Eff<TResult> eff)
        {
            switch (eff)
            {
                case ExceptionEff<TResult> setException:
                    throw setException.Exception;
                case ResultEff<TResult> setResult:
                    return new List<TResult> { setResult.Result };
                case DelayEff<TResult> delay:
                    return Run(delay.StateMachine.MoveNext());
                case AwaitEff<TResult> awaitEff:
                    var handler = new NonDetEffectHandler<TResult>(awaitEff.StateMachine);
                    awaitEff.Awaiter.Accept(handler).Wait();
                    return handler.Results;
                default:
                    throw new NotSupportedException($"{eff.GetType().Name}");
            }
        }
    }

    public class NonDetEffectHandler<TResult> : EffectHandler
    {
        private readonly EffStateMachine<TResult> _stateMachine;

        public NonDetEffectHandler(EffStateMachine<TResult> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public List<TResult> Results { get; } = new List<TResult>();

        public async override Task Handle<TValue>(EffectAwaiter<TValue> awaiter)
        {
            switch (awaiter.Effect)
            {
                case NonDetEffect<TValue> nde:
                    
                    foreach (var choice in nde.Choices)
                    {
                        awaiter.SetResult(choice);
                        var next = _stateMachine.Clone().MoveNext();
                        var results = NonDetEffectHandler.Run(next);
                        Results.AddRange(results);
                        awaiter.Clear();
                    }
                    break;
            }
        }
    }
}
