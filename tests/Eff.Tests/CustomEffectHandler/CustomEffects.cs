using System;

namespace Nessos.Effects.Tests
{
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
        public DateTimeNowEffect DateTimeNow() => new DateTimeNowEffect();

        public FuncEffect<TResult> Func<TResult>(Func<TResult> func) => new FuncEffect<TResult>(func);

        public FuncEffect<Unit> Action(Action action) => new FuncEffect<Unit>(() => { action(); return Unit.Value; });
    }
}
