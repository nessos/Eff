using Nessos.Effects.Handlers;
using Nessos.Effects.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nessos.Effects.Tests
{
    public class CustomEffectHandler : EffectHandler
    {
        private readonly DateTime _now;

        public List<ExceptionLog> ExceptionLogs { get; } = new List<ExceptionLog>();
        public List<ResultLog> TraceLogs { get; } = new List<ResultLog>();

        public CustomEffectHandler(DateTime now)
        {
            _now = now;
        }

        public CustomEffectHandler() : this(DateTime.Now)
        { }

        public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter)
            {
                case EffectAwaiter<DateTime> { Effect: DateTimeNowEffect _ } awtr:
                    awtr.SetResult(_now);
                    break;
                case { Effect: FuncEffect<TResult> funcEffect }:
                    var result = funcEffect.Func();
                    awaiter.SetResult(result);
                    break;
            }

            return default;
        }

        public (string name, object? value)[]? CaptureStateParameters { private set; get; }
        public (string name, object? value)[]? CaptureStateLocalVariables { private set; get; }

        public override async ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            await base.Handle(stateMachine);

            var awaitingStateMachine = stateMachine.AwaitingStateMachine?.GetAsyncStateMachine();
            if (awaitingStateMachine != null)
            {
                CaptureStateParameters = awaitingStateMachine.GetParameterValues();
                CaptureStateLocalVariables = awaitingStateMachine.GetLocalVariableValues();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                        Log(stateMachine.Result, stateMachine);
                        break;

                    case StateMachinePosition.Exception:
                        Log(stateMachine.Exception!, stateMachine);
                        break;
                }
            }
        }

        public void Log(Exception ex, EffAwaiter awaiter)
        {
            var stateMachine = awaiter.AwaitingStateMachine?.GetAsyncStateMachine()!;
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
            ExceptionLogs.Add(log);

            if (!ex.Data.Contains("StackTraceLog"))
            {
                var queue = new Queue<ExceptionLog>();
                queue.Enqueue(log);
                ex.Data["StackTraceLog"] = queue;
            }

            ((Queue<ExceptionLog>)ex.Data["StackTraceLog"]!).Enqueue(log);
        }

        public void Log(object? result, EffAwaiter awaiter)
        {
            var stateMachine = awaiter.AwaitingStateMachine?.GetAsyncStateMachine()!;
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