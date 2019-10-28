using System;
using System.Runtime.CompilerServices;

namespace Eff.Core
{
    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult>
    {

    }

    public class Await<TResult> : Eff<TResult>
    {
        public Await(IEffect effect, Func<object, Eff<TResult>> continuation, object state)
        {
            Effect = effect;
            Continuation = continuation;
            State = state;
        }

        public IEffect Effect { get; }
        public Func<object, Eff<TResult>> Continuation { get; }
        public object State { get; }
    }

    public class SetResult<TResult> : Eff<TResult>
    {
        public SetResult(TResult result, object state)
        {
            Result = result;
            State = state;
        }

        public TResult Result { get; }
        public object State { get; }
    }

    public class SetException<TResult> : Eff<TResult>
    {
        public SetException(Exception exception, object state)
        {
            Exception = exception;
            State = state;
        }

        public Exception Exception { get; }
        public object State { get; }
    }

    public class Delay<TResult> : Eff<TResult>
    {
        public Delay(Func<object, Eff<TResult>> func, object state)
        {
            Func = func;
            State = state;
        }

        public Func<object, Eff<TResult>> Func { get; }
        public object State { get; }
    }
}
