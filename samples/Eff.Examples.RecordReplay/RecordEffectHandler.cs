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
        public Type Type { get; set; }
        public object Value { get; set; }
    }

    public class RecordEffectHandler : EffectHandler
    {
        private readonly EffCtx ctx;
        public RecordEffectHandler(EffCtx ctx)
        {
            this.ctx = ctx;
        }

        private List<Result> results = new List<Result>();

        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DoEffect<TResult> _effect:
                    _effect.SetResult(_effect.Func(ctx));
                    break;
            }
            results.Add(new Result
            {
                FilePath = effect.CallerFilePath,
                MemberName = effect.CallerMemberName,
                LineNumber = effect.CallerLineNumber,
                Value = effect.Result,
                Type = typeof(TResult),
            });
        }

        public string GetJson() => JsonConvert.SerializeObject(results);
            
    }
}
