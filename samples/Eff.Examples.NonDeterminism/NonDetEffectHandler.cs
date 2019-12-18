#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.NonDeterminism
{
    public class NonDetHandler<TResult> : EffectHandler
    {
        private readonly IContinuation<TResult> _continuation;

        public NonDetHandler(IContinuation<TResult> continuation)
        {
            _continuation = continuation;
        }

        public List<TResult> Results { get; } = new List<TResult>();

        public async override Task Handle<TValue>(IEffect<TValue> effect)
        {
            switch (effect)
            {
                case NonDetEffect<TValue> nde:
                    
                    foreach (var choice in nde.Choices)
                    {
                        effect.SetResult(choice);
                        var results = Effect.Run(_continuation.Trigger(useClonedStateMachine: true));
                        Results.AddRange(results);
                    }
                    break;
            }
        }
    }
}
