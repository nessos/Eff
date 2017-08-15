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
        private readonly List<TResult> results = new List<TResult>();
        private readonly Func<object, Eff<TResult>> continuation;

        public List<TResult> Results => results;

        public NonDetHandler(Func<object, Eff<TResult>> continuation)
        {
            this.continuation = continuation;
        }

        private static object Clone(object obj) 
        {
            MethodInfo info = obj.GetType().GetMethod("MemberwiseClone",
                BindingFlags.Instance | BindingFlags.NonPublic);
            
            return info.Invoke(obj, null);
            
        }

        public async override ValueTask<ValueTuple> Handle<TValue>(IEffect<TValue> effect)
        {
            switch (effect)
            {
                case NonDetEffect<TValue> _effect:
                    
                    foreach (var choice in _effect.Choices)
                    {
                        effect.SetResult(choice);
                        var state = _effect.State;
                        object _state = Clone(state);
                        var result = Effect.Run(continuation(_state));
                        results.AddRange(result);
                    }
                    break;
            }
            
            return ValueTuple.Create();
        }
    }
}
