﻿#pragma warning disable 1998
using Nessos.Effects.Handlers;
using Nessos.Effects.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.StackTrace
{
    public class CustomEffectHandler : EffectHandler
    {
        public override async Task Handle<TResult>(EffectAwaiter<TResult> effect) { }

        public override async Task Handle<TResult>(TaskAwaiter<TResult> effect)
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
        }

        public override async Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            await base.Handle(stateMachine);

            if (stateMachine.Exception is Exception ex)
            {
                await Log(ex, stateMachine);
            }
        }

        public async Task Log(Exception ex, Awaiter awaiter)
        {
            var stateMachine = awaiter.StateMachine?.GetAsyncStateMachine()!;

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
