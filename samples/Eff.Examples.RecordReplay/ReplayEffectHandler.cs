#pragma warning disable 1998
using Nessos.Eff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    public class ReplayEffectHandler : EffectHandler
    {

        private readonly List<Result> results;

        public ReplayEffectHandler(List<Result> results)
        {
            this.results = results;
        }

        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            var result = 
                results.Single(it => it.FilePath == effect.CallerFilePath && 
                                     it.MemberName == effect.CallerMemberName && 
                                     it.LineNumber == effect.CallerLineNumber);
            effect.SetResult((TResult)Convert.ChangeType(result.Value, result.Type));
        }
    }
}
