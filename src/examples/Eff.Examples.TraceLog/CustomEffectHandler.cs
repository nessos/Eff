#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.TraceLog
{
    public class CustomEffectHandler : EffectHandler
    {

        public CustomEffectHandler() 
            : base(enableTraceLogging : true, 
                   enableParametersLogging : true, 
                   enableLocalVariablesLogging : true)
        {
            
        }


        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Log(ExceptionLog log)
        {
            return ValueTuple.Create();
        }

        public List<ResultLog> TraceLogs = new List<ResultLog>();
        public override async ValueTask<ValueTuple> Log(ResultLog log)
        {
            TraceLogs.Add(log);
            return ValueTuple.Create();
        }
    }
}
