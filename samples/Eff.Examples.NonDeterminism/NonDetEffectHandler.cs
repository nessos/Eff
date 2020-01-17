#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Eff.Examples.NonDeterminism
{
    public static class NonDetEffectHandler
    {
        public static List<TResult> Run<TResult>(Eff<TResult> eff)
        {
            switch (eff)
            {
                case SetException<TResult> setException:
                    throw setException.Exception;
                case SetResult<TResult> setResult:
                    return new List<TResult> { setResult.Result };
                case Delay<TResult> delay:
                    return Run(delay.Continuation.Trigger());
                case Await<TResult> awaitEff:
                    var handler = new NonDetEffectHandler<TResult>(awaitEff.Continuation);
                    awaitEff.Awaiter.Accept(handler);
                    return handler.Results;
                default:
                    throw new NotSupportedException($"{eff.GetType().Name}");
            }
        }
    }

    public class NonDetEffectHandler<TResult> : EffectHandler
    {
        private readonly IContinuation<TResult> _continuation;

        public NonDetEffectHandler(IContinuation<TResult> continuation)
        {
            _continuation = continuation;
        }

        public List<TResult> Results { get; } = new List<TResult>();

        public async override Task Handle<TValue>(EffectEffAwaiter<TValue> awaiter)
        {
            switch (awaiter.Effect)
            {
                case NonDetEffect<TValue> nde:
                    
                    foreach (var choice in nde.Choices)
                    {
                        awaiter.SetResult(choice);
                        var next = _continuation.Trigger(useClonedStateMachine: true);
                        var results = NonDetEffectHandler.Run(next);
                        Results.AddRange(results);
                    }
                    break;
            }
        }
    }
}
