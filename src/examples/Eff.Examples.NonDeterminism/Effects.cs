using Eff.Core;
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
        public static NonDetEffect<T> Choose<T>(T[] choices,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new NonDetEffect<T>(choices, memberName, sourceFilePath, sourceLineNumber, captureState);
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
                    return Run(delay.Func(delay.State));
                case Await<TResult> awaitEff:
                    var handler = new NonDetHandler<TResult>(awaitEff.Continuation);
                    var effect = awaitEff.Effect;
                    var _ = effect.Accept(handler).Result;
                    return handler.Results;
                default:
                    throw new NotSupportedException($"{eff.GetType().Name}");
            }
        }
    }
}
