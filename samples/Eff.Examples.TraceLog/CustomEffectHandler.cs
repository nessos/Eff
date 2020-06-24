#pragma warning disable 1998
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.Utils;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Examples.TraceLog
{
    public class CustomEffectHandler : EffectHandler
    {
        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {

        }

        public override async Task Handle<TResult>(TaskAwaiter<TResult> awaiter)
        {
            await base.Handle(awaiter);

            if (awaiter.HasResult)
            {
                await Log(awaiter.Result, awaiter);
            }
        }

        public override async Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            await base.Handle(stateMachine);

            if (stateMachine.HasResult)
            {
                await Log(stateMachine.Result, stateMachine);
            }
        }

        public List<ResultLog> TraceLogs = new List<ResultLog>();
        public async Task Log(object? result, Awaiter awaiter)
        {
            var stateMachine = awaiter.StateMachine?.GetAsyncStateMachine()!;
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
