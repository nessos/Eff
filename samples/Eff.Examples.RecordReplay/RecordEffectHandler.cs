#pragma warning disable 1998
using Nessos.Eff;
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

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter.Effect)
            {
                case DoEffect<TResult> doEffect:
                    awaiter.SetResult(doEffect.Func(ctx));
                    break;
            }
            results.Add(new Result
            {
                FilePath = awaiter.CallerFilePath,
                MemberName = awaiter.CallerMemberName,
                LineNumber = awaiter.CallerLineNumber,
                Value = awaiter.Result,
                Type = typeof(TResult),
            });
        }

        public string GetJson() => JsonConvert.SerializeObject(results);
            
    }
}
