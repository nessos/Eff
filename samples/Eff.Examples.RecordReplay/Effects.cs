using System;

namespace Nessos.Effects.Examples.RecordReplay
{

    public class EffCtx
    {
        public Random Random { get; set; }
    }

    public class DoEffect<T> : Effect<T>
    {
        public Func<EffCtx, T> Func { get; }

        public DoEffect(Func<EffCtx, T> func)
        {
            Func = func;
        }
    }

    public static class IO
    {
        public static DoEffect<T> Do<T>(Func<EffCtx, T> func) => new DoEffect<T>(func);
    }
}