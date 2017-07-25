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

        public override async ValueTask<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff)
        {
            var eff = await base.Handle(awaitEff);
            var effect = awaitEff.Effect;
            if (effect.HasResult)
            {
                await Log(new ResultLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Result = effect.Result,
                    Parameters = Eff.Core.Utils.GetParametersValues(awaitEff.State),
                    LocalVariables = Eff.Core.Utils.GetLocalVariablesValues(awaitEff.State),
                });
            }

            return eff;
        }

        public List<ResultLog> TraceLogs = new List<ResultLog>();
        public async ValueTask<ValueTuple> Log(ResultLog log)
        {
            TraceLogs.Add(log);
            return ValueTuple.Create();
        }
    }
}
