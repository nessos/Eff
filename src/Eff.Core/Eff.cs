using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Nessos.Eff
{
    [AsyncMethodBuilder(typeof(EffMethodBuilder))]
    public abstract class Eff
    {
        internal Eff() { }

        public EffAwaiter ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                      [CallerFilePath] string callerFilePath = "",
                                      [CallerLineNumber] int callerLineNumber = 0)
        {
            var awaiter = GetAwaiterCore();
            awaiter.CallerMemberName = callerMemberName;
            awaiter.CallerFilePath = callerFilePath;
            awaiter.CallerLineNumber = callerLineNumber;
            return awaiter;
        }

        public EffAwaiter GetAwaiter() => GetAwaiterCore();

        internal abstract Task Accept(IEffectHandler handler);
        internal abstract EffAwaiter GetAwaiterCore();
    }

    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult> : Eff
    {
        internal Eff() { }

        public new EffAwaiter<TResult> ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                                      [CallerFilePath] string callerFilePath = "",
                                                      [CallerLineNumber] int callerLineNumber = 0)
        {
            return new EffEffAwaiter<TResult>(this) 
            { 
                CallerMemberName = callerMemberName, 
                CallerFilePath = callerFilePath, 
                CallerLineNumber = callerLineNumber 
            };
        }

        public new EffAwaiter<TResult> GetAwaiter() => new EffEffAwaiter<TResult>(this);

        internal override Task Accept(IEffectHandler handler) => this.Run(handler);
        internal override EffAwaiter GetAwaiterCore() => GetAwaiter();
    }

    public class Await<TResult> : Eff<TResult>
    {
        public Await(EffAwaiter awaiter, IContinuation<TResult> continuation)
        {
            Awaiter = awaiter;
            Continuation = continuation;
        }

        public EffAwaiter Awaiter { get; }
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
