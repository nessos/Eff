using Nessos.Effects.Handlers;
using Nessos.Effects.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.StackTrace
{
    public class CustomEffectHandler : EffectHandler
    {
        public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            return default;
        }

        public override async ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            await base.Handle(stateMachine);

            if (stateMachine.Exception is Exception ex)
            {
                Log(ex, stateMachine);
            }
        }

        public void Log(Exception ex, EffAwaiter awaiter)
        {
            var stateMachine = awaiter.AwaitingStateMachine?.GetAsyncStateMachine();
            if (stateMachine is null)
            {
                return;
            }

            var log =
                new ExceptionLog
                {
                    CallerFilePath = awaiter.CallerFilePath,
                    CallerLineNumber = awaiter.CallerLineNumber,
                    CallerMemberName = awaiter.CallerMemberName,
                    Exception = ex,
                    Parameters = stateMachine.GetParameterValues(),
                    LocalVariables = stateMachine.GetLocalVariableValues(),
                };
            if (!ex.Data.Contains("StackTraceLog"))
            {
                var queue = new Queue<ExceptionLog>();
                queue.Enqueue(log);
                ex.Data["StackTraceLog"] = queue;
                return;
            }

            ((Queue<ExceptionLog>)ex.Data["StackTraceLog"]!).Enqueue(log);
        }
    }
}
