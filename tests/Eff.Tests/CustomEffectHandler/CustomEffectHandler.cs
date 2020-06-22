#pragma warning disable 1998

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

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
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
        }

        public (string name, object? value)[]? CaptureStateParameters { private set; get; }
        public (string name, object? value)[]? CaptureStateLocalVariables { private set; get; }

        public override async Task Handle<TResult>(TaskAwaiter<TResult> awaiter)
        {
            var stateMachine = awaiter.StateMachine?.GetStateMachine()!;
            CaptureStateParameters = stateMachine.GetParameterValues();
            CaptureStateLocalVariables = stateMachine.GetLocalVariableValues();

            try
            {
                var result = await awaiter.Task;
                awaiter.SetResult(result);
                await Log(result, awaiter);
            }
            catch (Exception ex)
            {
                await Log(ex, awaiter);
                throw;
            }

        }

        public override async Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            await base.Handle(stateMachine);
            if (stateMachine.StateMachine != null)
            {
                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                        await Log(stateMachine.Result, stateMachine);
                        break;

                    case StateMachinePosition.Exception:
                        await Log(stateMachine.Exception!, stateMachine);
                        break;
                }
            }
        }

        public async ValueTask<ValueTuple> Log(Exception ex, Awaiter awaiter)
        {
            var stateMachine = awaiter.StateMachine?.GetStateMachine()!;
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

                return ValueTuple.Create();
            }

            ((Queue<ExceptionLog>)ex.Data["StackTraceLog"]!).Enqueue(log);

            return ValueTuple.Create();
        }

        public async ValueTask<ValueTuple> Log(object? result, Awaiter awaiter)
        {
            var stateMachine = awaiter.StateMachine?.GetStateMachine()!;
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
            return ValueTuple.Create();
        }
    }
}