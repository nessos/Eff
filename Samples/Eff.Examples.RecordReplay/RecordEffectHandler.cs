#pragma warning disable 1998
using Eff.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    public class Result
    {
        public string FilePath { get; set; }
        public string MemberName { get; set; }
        public int LineNumber { get; set; }
        public object Value { get; set; }
    }

    public class RecordEffectHandler : EffectHandler
    {

        private List<Result> results = new List<Result>();
        private Random random = new Random();

        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DateTimeNowEffect _effect:
                    _effect.SetResult(DateTime.Now);
                    break;
                case RandomEffect _effect:
                    _effect.SetResult(random.Next());
                    break;
            }
            results.Add(new Result
            {
                FilePath = effect.CallerFilePath,
                MemberName = effect.CallerMemberName,
                LineNumber = effect.CallerLineNumber,
                Value = effect.Result
            });
        }

        public string GetJson() => JsonConvert.SerializeObject(results);
            
    }
}
