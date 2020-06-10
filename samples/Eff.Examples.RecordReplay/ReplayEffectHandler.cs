#pragma warning disable 1998
using Nessos.Effects.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.RecordReplay
{
    public class ReplayEffectHandler : EffectHandler
    {
        private readonly RecordedResult[] _results;
        private int _index = 0;

        public ReplayEffectHandler(IEnumerable<RecordedResult> results)
        {
            _results = results.ToArray();
        }

        public override async Task Handle<TResult>(EffectAwaiter<TResult> effect)
        {
            if (_index == _results.Length)
            {
                throw new InvalidOperationException();
            }

            var result = _results[_index++];
            result.ToAwaiter(effect);
        }
    }
}
