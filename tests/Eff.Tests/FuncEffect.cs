using Nessos.Eff;
using System;

namespace Nessos.Eff.Tests
{
    public class FuncEffect<TResult> : Effect<TResult>
    {
        public FuncEffect(Func<TResult> func)
        {
            Func = func;
        }

        public Func<TResult> Func { get; }
    }
}