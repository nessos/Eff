namespace Nessos.Effects.Examples.TraceLog;

using Nessos.Effects.Handlers;
using Nessos.Effects.Utils;

public class CustomEffectHandler : EffectHandler
{
    public List<ResultLog> TraceLogs = [];

    public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter) => default;

    public override async ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine)
    {
        await base.Handle(stateMachine);

        if (stateMachine.HasResult)
        {
            Log(stateMachine.Result, stateMachine);
        }
    }

    public void Log(object? result, EffAwaiter awaiter)
    {
        var stateMachine = awaiter.AwaitingStateMachine?.GetAsyncStateMachine();

        if (stateMachine is null)
        {
            return;
        }

        var log = new ResultLog
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
