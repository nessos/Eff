using Nessos.Eff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.NonDeterminism
{

    public static class Effect
    {
        public static NonDetEffect<T> Choose<T>(params T[] choices)
        {
            return new NonDetEffect<T>(choices);
        }

        public static List<TResult> Run<TResult>(this Eff<TResult> eff)
        {
            switch (eff)
            {
                case SetException<TResult> setException:
                    throw setException.Exception;
                case SetResult<TResult> setResult:
                    return new List<TResult> { setResult.Result };
                case Delay<TResult> delay:
                    return Run(delay.Continuation.Trigger());
                case Await<TResult> awaitEff:
                    var handler = new NonDetHandler<TResult>(awaitEff.Continuation);
                    awaitEff.Awaiter.Accept(handler);
                    return handler.Results;
                default:
                    throw new NotSupportedException($"{eff.GetType().Name}");
            }
        }
    }
}
