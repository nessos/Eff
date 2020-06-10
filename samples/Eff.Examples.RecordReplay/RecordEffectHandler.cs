#pragma warning disable 1998
using Nessos.Effects.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.RecordReplay
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
        private readonly EffCtx _ctx;
        private readonly List<Result> _results = new List<Result>();

        public RecordEffectHandler(EffCtx ctx)
        {
            _ctx = ctx;
        }

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter.Effect)
            {
                case DoEffect<TResult> doEffect:
                    awaiter.SetResult(doEffect.Func(_ctx));
                    break;
            }

            _results.Add(new Result
            {
                FilePath = awaiter.CallerFilePath,
                MemberName = awaiter.CallerMemberName,
                LineNumber = awaiter.CallerLineNumber,
                Value = awaiter.Result,
                Type = typeof(TResult),
            });
        }

        public string GetJson() => JsonConvert.SerializeObject(_results);
            
    }
}
