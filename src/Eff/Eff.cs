using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Effects
{
    /// <summary>
    ///   Represents an effectful computation built using the Eff library.
    ///   Execution of the computation is delayed (a.k.a. "cold semantics").
    ///   To start an Eff computation, an effect handler must by supplied using the Eff.Run() method.
    /// </summary>
    [AsyncMethodBuilder(typeof(EffMethodBuilder))]
    public abstract class Eff
    {
        internal Eff() { }

        /// <summary>
        ///   Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public Awaiter ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
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
        ///   Implements the awaitable/awaiter pattern for Eff.
        /// </summary>
        public Awaiter GetAwaiter() => GetAwaiterCore();

        /// <summary>
        ///   Runs supplied Eff computation using provided effect handler.
        /// </summary>
        /// <param name="handler">Effect handler to be used in execution.</param>
        public Task Run(IEffectHandler handler) => RunCore(handler);
        
        // Helper methods for exposing untyped variants of Run and GetAwaiter methods
        // Can be removed once Covariant return types are brought to C#.
        protected abstract Task RunCore(IEffectHandler handler);
        protected abstract Awaiter GetAwaiterCore();
    }

    /// <summary>
    ///   Represents an effectful computation built using the Eff library.
    ///   Execution of the computation is delayed (a.k.a. "cold semantics").
    ///   To start an Eff computation, an effect handler must by supplied using the Eff.Run() method.
    /// </summary>
    /// <typeparam name="TResult">The return type of the computation.</typeparam>
    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult> : Eff
    {
        internal Eff() { }

        /// <summary>
        ///   Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public new EffStateMachine<TResult> ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var awaiter = GetAwaiter();
            awaiter.CallerMemberName = callerMemberName;
            awaiter.CallerFilePath = callerFilePath;
            awaiter.CallerLineNumber = callerLineNumber;
            return awaiter;
        }

        /// <summary>
        ///   Implements the awaitable/awaiter pattern for Eff
        /// </summary>
        public new abstract EffStateMachine<TResult> GetAwaiter();

        /// <summary>
        ///   Runs supplied Eff computation using provided effect handler.
        /// </summary>
        /// <param name="handler">Effect handler to be used in execution.</param>
        /// <returns></returns>
        public new async Task<TResult> Run(IEffectHandler handler)
        {
            var stateMachine = GetAwaiter();
            await handler.Handle(stateMachine);
            return stateMachine.Result;
        }

        protected override Task RunCore(IEffectHandler handler) => Run(handler);
        protected override Awaiter GetAwaiterCore() => GetAwaiter();
    }
}
