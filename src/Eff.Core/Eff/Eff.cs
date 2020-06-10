using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Nessos.Effects
{
    /// <summary>
    /// Represents an effectful computation built using the Eff library.
    /// Execution of the computation is delayed (a.k.a. "cold semantics").
    /// To start an Eff computation, an effect handler must by supplied using the Eff.Run() method.
    /// </summary>
    [AsyncMethodBuilder(typeof(EffMethodBuilder))]
    public abstract class Eff
    {
        internal Eff() { }

        /// <summary>
        /// Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public EffAwaiterBase ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            var awaiter = GetAwaiterCore();
            awaiter.CallerMemberName = callerMemberName;
            awaiter.CallerFilePath = callerFilePath;
            awaiter.CallerLineNumber = callerLineNumber;
            return awaiter;
        }

        /// <summary>
        /// Implements the awaitable/awaiter pattern for Eff
        /// </summary>
        public EffAwaiterBase GetAwaiter() => GetAwaiterCore();

        /// <summary>
        /// Helper method for interpreting untyped Eff instances
        /// </summary>
        internal abstract Task RunCore(IEffectHandler handler);

        /// <summary>
        /// Helper method for untyped GetAwaiter() instances
        /// </summary>
        internal abstract EffAwaiterBase GetAwaiterCore();
    }

    /// <summary>
    /// Represents an effectful computation built using the Eff library.
    /// Execution of the computation is delayed (a.k.a. "cold semantics").
    /// To start an Eff computation, an effect handler must by supplied using the Eff.Run() method.
    /// </summary>
    /// <typeparam name="TResult">The return type of the computation.</typeparam>
    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult> : Eff
    {
        internal Eff() { }

        /// <summary>
        /// Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public new EffAwaiterBase<TResult> ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                                      [CallerFilePath] string callerFilePath = "",
                                                      [CallerLineNumber] int callerLineNumber = 0)
        {
            return new EffAwaiter<TResult>(this) 
            { 
                CallerMemberName = callerMemberName, 
                CallerFilePath = callerFilePath, 
                CallerLineNumber = callerLineNumber 
            };
        }

        /// <summary>
        /// Implements the awaitable/awaiter pattern for Eff
        /// </summary>
        public new EffAwaiterBase<TResult> GetAwaiter() => new EffAwaiter<TResult>(this);

        internal override Task RunCore(IEffectHandler handler) => this.Run(handler);
        internal override EffAwaiterBase GetAwaiterCore() => GetAwaiter();
    }

    /// <summary>
    /// Eff instance representing a delayed computation.
    /// </summary>
    public class DelayEff<TResult> : Eff<TResult>
    {
        internal DelayEff(IEffStateMachine<TResult> continuation)
        {
            Continuation = continuation;
        }

        public IEffStateMachine<TResult> Continuation { get; }
    }

    /// <summary>
    /// Eff instance representing an await-ed computation
    /// </summary>
    public class AwaitEff<TResult> : Eff<TResult>
    {
        internal AwaitEff(EffAwaiterBase awaiter, IEffStateMachine<TResult> continuation)
        {
            Awaiter = awaiter;
            Continuation = continuation;
        }

        public EffAwaiterBase Awaiter { get; }
        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public IEffStateMachine<TResult> Continuation { get; }
    }

    /// <summary>
    /// Eff instance representing a completed computation.
    /// </summary>
    public class ResultEff<TResult> : Eff<TResult>
    {
        internal ResultEff(TResult result, object state)
        {
            Result = result;
            State = state;
        }

        /// <summary>
        /// Materialized result of the computation
        /// </summary>
        public TResult Result { get; }
        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public object State { get; }
    }

    /// <summary>
    /// Eff instance representing an exceptional computation.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ExceptionEff<TResult> : Eff<TResult>
    {
        internal ExceptionEff(Exception exception, object state)
        {
            Exception = exception;
            State = state;
        }

        /// <summary>
        /// Materialized exception of the computation
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        public object State { get; }
    }

    /// <summary>
    /// Represents an Eff state machine that can be triggered on demand.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IEffStateMachine<TResult>
    {
        /// <summary>
        ///  The current state object of the machine.
        /// </summary>
        object State { get; }

        /// <summary>
        /// Advances the state machine to its next stage.
        /// </summary>
        /// <param name="useClonedStateMachine">
        /// Use a clone of the state machine graph when executing. 
        /// Useful in applications requiring referential transparency. 
        /// Defaults to false for performance.
        /// </param>
        /// <returns>An Eff instance representing the next stage of the computation.</returns>
        Eff<TResult> MoveNext(bool useClonedStateMachine = false);
    }
}
