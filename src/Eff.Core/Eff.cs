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
        public Await(IEffect effect, IContinuation<TResult> continuation)
        {
            Effect = effect;
            Continuation = continuation;
        }

        public IEffect Effect { get; }
        public IContinuation<TResult> Continuation { get; }
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
        public Delay(IContinuation<TResult> continuation)
        {
            Continuation = continuation;
        }

        public IContinuation<TResult> Continuation { get; }
    }

    public interface IContinuation<TResult>
    {
        object State { get; set; }
        Eff<TResult> Trigger(bool useClonedStateMachine = false);
    }
}
