#pragma warning disable 1998
using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.StackTrace
{
    public class CustomEffectHandler : EffectHandler
    {

        public CustomEffectHandler() 
            : base(enableExceptionLogging : true, 
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
            var ex = log.Exception;
            if (!ex.Data.Contains("StackTraceLog"))
            {
                var queue = new Queue<ExceptionLog>();
                queue.Enqueue(log);
                ex.Data["StackTraceLog"] = queue;

                return ValueTuple.Create();
            }

            ((Queue<ExceptionLog>)ex.Data["StackTraceLog"]).Enqueue(log);

            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Log(ResultLog log)
        {
            
            return ValueTuple.Create();
        }
    }
}
