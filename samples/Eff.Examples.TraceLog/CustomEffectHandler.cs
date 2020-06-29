#pragma warning disable 1998
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.Utils;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Examples.TraceLog
{
    public class CustomEffectHandler : EffectHandler
    {
        public List<ResultLog> TraceLogs = new List<ResultLog>();

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {

        }

        public override async Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            await base.Handle(stateMachine);

            if (stateMachine.HasResult)
            {
                await Log(stateMachine.Result, stateMachine);
            }
        }

        public async Task Log(object? result, EffAwaiter awaiter)
        {
            var stateMachine = awaiter.AwaitingStateMachine?.GetAsyncStateMachine();

            if (stateMachine is null)
            {
                return;
            }

            var log =
                new ResultLog
                {
                    Result = result,
                    CallerFilePath = awaiter.CallerFilePath,
                    CallerLineNumber = awaiter.CallerLineNumber,
                    CallerMemberName = awaiter.CallerMemberName,
                    Parameters = stateMachine.GetParameterValues(),
                    LocalVariables = stateMachine.GetLocalVariableValues(),
                };

            TraceLogs.Add(log);
        }
    }
}
