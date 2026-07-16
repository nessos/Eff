namespace Nessos.Effects.Tests;

public interface IDateTimeNowEffect
{
    DateTimeNowEffect DateTimeNow();
}

public interface IFuncEffect
{
    FuncEffect<TResult> Func<TResult>(Func<TResult> func);

    FuncEffect<Unit> Action(Action action);
}
public struct CustomEffect : IDateTimeNowEffect, IFuncEffect
{
    public DateTimeNowEffect DateTimeNow() => new();

    public FuncEffect<TResult> Func<TResult>(Func<TResult> func) => new(func);

    public FuncEffect<Unit> Action(Action action) => new(() => { action(); return Unit.Value; });
}
