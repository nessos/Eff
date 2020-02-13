#pragma warning disable 1998
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nessos.Eff.Examples.RecordReplay
{
    public class ReplayEffectHandler : EffectHandler
    {

        private readonly List<Result> _results;

        public ReplayEffectHandler(List<Result> results)
        {
            _results = results;
        }

        public static ReplayEffectHandler FromJson(string json)
        {
            return new ReplayEffectHandler(JsonConvert.DeserializeObject<List<Result>>(json));
        }

        public override async Task Handle<TResult>(EffectAwaiter<TResult> effect)
        {
            var result = 
                _results.Single(it => it.FilePath == effect.CallerFilePath && 
                                     it.MemberName == effect.CallerMemberName && 
                                     it.LineNumber == effect.CallerLineNumber);
            effect.SetResult((TResult)Convert.ChangeType(result.Value, result.Type));
        }
    }
}
