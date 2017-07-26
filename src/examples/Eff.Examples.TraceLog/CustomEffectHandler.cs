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
            : base()
        {
            
        }


        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            
            var result = await effect.Task;
            effect.SetResult(result);
            await Log(result, effect);

            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Handle<TResult>(EffEffect<TResult> effect)
        {

            var result = await effect.Eff.Run(this);
            effect.SetResult(result);
            await Log(result, effect);

            return ValueTuple.Create();
        }

        public List<ResultLog> TraceLogs = new List<ResultLog>();
        public async ValueTask<ValueTuple> Log(object result, IEffect effect)
        {
            var log =
                new ResultLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Result = result,
                    Parameters = Eff.Core.Utils.GetParametersValues(effect.State),
                    LocalVariables = Eff.Core.Utils.GetLocalVariablesValues(effect.State),
                };
            TraceLogs.Add(log);
            return ValueTuple.Create();
        }
    }
}
