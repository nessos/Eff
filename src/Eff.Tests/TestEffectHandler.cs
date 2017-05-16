using Eff.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TestEffectHandler : EffectHandler
    {
        private readonly DateTime now;

        public List<ExceptionLog> ExceptionLogs { get; }
        public List<ResultLog> TraceLogs { get; }

        public TestEffectHandler(DateTime now) : base(true, true, true, true)
        {
            this.now = now;
            ExceptionLogs = new List<ExceptionLog>();
            TraceLogs = new List<ResultLog>();
        }

        public TestEffectHandler() : this(DateTime.Now)
        { }

        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DateTimeNowEffect dateTimeNowEffect:
                    dateTimeNowEffect.SetResult(now);
                    break;
            }

            return ValueTuple.Create();
        }

        

        public override async ValueTask<ValueTuple> Log(ExceptionLog log)
        {
            ExceptionLogs.Add(log);
            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Log(ResultLog log)
        {
            TraceLogs.Add(log);
            return ValueTuple.Create();
        }
    }


}
