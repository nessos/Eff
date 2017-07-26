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
            : base()
        {
            
        }


        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            try
            {
                var result = await effect.Task;
                effect.SetResult(result);
            }
            catch (Exception ex)
            {
                await Log(ex, effect);
                throw;
            }

            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(EffEffect<TResult> effect)
        {
            try
            {
                var result = await effect.Eff.Run(this);
                effect.SetResult(result);
            }
            catch (Exception ex)
            {
                await Log(ex, effect);
                throw;
            }

            return ValueTuple.Create();
        }


        public async ValueTask<ValueTuple> Log(Exception ex, IEffect effect)
        {
            var log =
                new ExceptionLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Exception = ex,
                    Parameters = Eff.Core.Utils.GetParametersValues(effect.State),
                    LocalVariables = Eff.Core.Utils.GetLocalVariablesValues(effect.State),
                };
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
    }
}
