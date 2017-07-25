#pragma warning disable 1998

using Eff.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TestEffectHandler : EffectHandler
    {
        private readonly DateTime now;

        public List<ExceptionLog> ExceptionLogs { get; }
        public List<ResultLog> TraceLogs { get; }

        public TestEffectHandler(DateTime now) : base()
        {
            this.now = now;
            ExceptionLogs = new List<ExceptionLog>();
            TraceLogs = new List<ResultLog>();
        }

        public TestEffectHandler() : this(DateTime.Now)
        { }

        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DateTimeNowEffect dateTimeNowEffect:
                    dateTimeNowEffect.SetResult(now);
                    break;
                case FuncEffect<TResult> funcEffect:
                    var result = funcEffect.Func();
                    effect.SetResult(result);
                    break;
            }

            return ValueTuple.Create();
        }

        public (string name, object value)[] CaptureStateParameters { private set; get; }
        public (string name, object value)[] CaptureStateLocalVariables { private set; get; }
        public override async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            if (effect.CaptureState)
            {
                CaptureStateParameters = effect.Parameters;
                CaptureStateLocalVariables = effect.LocalVariables;
            }

            var result = await effect.Task;
            effect.SetResult(result);

            return ValueTuple.Create();
        }

        public override async ValueTask<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff)
        {
            var eff = await base.Handle(awaitEff);
            var effect = awaitEff.Effect;
            if (effect.Exception != null)
            {
                await Log(new ExceptionLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Exception = effect.Exception,
                    Parameters = Utils.GetParametersValues(awaitEff.State),
                    LocalVariables = Utils.GetLocalVariablesValues(awaitEff.State),
                });
            }
            if (effect.HasResult)
            {
                await Log(new ResultLog
                {
                    CallerFilePath = effect.CallerFilePath,
                    CallerLineNumber = effect.CallerLineNumber,
                    CallerMemberName = effect.CallerMemberName,
                    Result = effect.Result,
                    Parameters = Utils.GetParametersValues(awaitEff.State),
                    LocalVariables = Utils.GetLocalVariablesValues(awaitEff.State),
                });
            }

            return eff;
        }

        public async ValueTask<ValueTuple> Log(ExceptionLog log)
        {
            ExceptionLogs.Add(log);

            var ex = log.Exception;
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

        public async ValueTask<ValueTuple> Log(ResultLog log)
        {
            TraceLogs.Add(log);
            return ValueTuple.Create();
        }
    }


}
