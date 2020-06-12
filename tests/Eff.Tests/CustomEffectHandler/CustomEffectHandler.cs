#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.Handlers;
using Nessos.Effects.Utils;

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

        public (string name, object value)[] CaptureStateParameters { private set; get; }
        public (string name, object value)[] CaptureStateLocalVariables { private set; get; }
        public override async Task Handle<TResult>(TaskAwaiter<TResult> effect)
        {
            CaptureStateParameters = TraceHelpers.GetParametersValues(effect.State);
            CaptureStateLocalVariables = TraceHelpers.GetLocalVariablesValues(effect.State);

            try
            {
                var result = await effect.Task;
                effect.SetResult(result);
                await Log(result, effect);
            }
            catch (Exception ex)
            {
                await Log(ex, effect);
                throw;
            }

        }

        public override async Task Handle<TResult>(EffAwaiter<TResult> effect)
        {
            try
            {
                var result = await effect.Eff.Run(this);
                effect.SetResult(result);
                await Log(result, effect);
            }
            catch (Exception ex)
            {
                await Log(ex, effect);
                throw;
            }
        }

        public async ValueTask<ValueTuple> Log(Exception ex, EffAwaiterBase effect)
        {
            var log =
                new ExceptionLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Exception = ex,
                    Parameters = TraceHelpers.GetParametersValues(effect.State),
                    LocalVariables = TraceHelpers.GetLocalVariablesValues(effect.State),
                };
            ExceptionLogs.Add(log);

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

        public async ValueTask<ValueTuple> Log(object result, EffAwaiterBase effect)
        {
            var log =
                new ResultLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Result = result,
                    Parameters = TraceHelpers.GetParametersValues(effect.State),
                    LocalVariables = TraceHelpers.GetLocalVariablesValues(effect.State),
                };
            TraceLogs.Add(log);
            return ValueTuple.Create();
        }
    }
}