using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Nessos.Eff
{

    [AsyncMethodBuilder(typeof(EffMethodBuilder))]
    public abstract class Eff
    {
        internal Eff() { }

        internal abstract Eff<Unit> Ignore();
    }

    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult> : Eff
    {
        internal Eff() { }

        internal async override Eff<Unit> Ignore() { await this.AsEffect(); return Unit.Value; }
    }

    public class Await<TResult> : Eff<TResult>
    {
        public Await(Awaiter awaiter, IContinuation<TResult> continuation)
        {
            Awaiter = awaiter;
            Continuation = continuation;
        }

        public Awaiter Awaiter { get; }
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
