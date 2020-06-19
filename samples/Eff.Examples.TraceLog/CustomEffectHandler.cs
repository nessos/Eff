#pragma warning disable 1998
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.Utils;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Examples.TraceLog
{
    public class CustomEffectHandler : EffectHandler
    {
        public override async Task Handle<TResult>(EffectAwaiter<TResult> effect)
        {

        }

        public override async Task Handle<TResult>(TaskAwaiter<TResult> effect)
        {            
            var result = await effect.Task;
            effect.SetResult(result);
            await Log(result, effect);
        }

        public override async Task Handle<TResult>(EffAwaiter<TResult> effect)
        {
            var result = await effect.Eff.Run(this);
            effect.SetResult(result);
            await Log(result, effect);
        }

        public List<ResultLog> TraceLogs = new List<ResultLog>();
        public async Task Log(object? result, Awaiter awaiter)
        {
            var stateMachine = awaiter.AwaitingEvaluator?.GetStateMachine()!;
            var log =
                new ResultLog
                {
                    CallerFilePath = awaiter.CallerFilePath,
                    CallerLineNumber = awaiter.CallerLineNumber,
                    CallerMemberName = awaiter.CallerMemberName,
                    Result = result,
                    Parameters = stateMachine.GetParameterValues(),
                    LocalVariables = stateMachine.GetLocalVariableValues(),
                };

            TraceLogs.Add(log);
        }
    }
}
