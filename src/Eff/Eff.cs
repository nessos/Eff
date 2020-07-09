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
    public abstract partial class Eff
    {
        internal Eff() { }

        /// <summary>
        ///   Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to the method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number at the source file at which the method is called.</param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public EffAwaiter ConfigureAwait(
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
        public EffAwaiter GetAwaiter() => GetAwaiterCore();

        /// <summary>
        ///    Executes the Eff computation using semantics from the provided effect handler.
        /// </summary>
        /// <param name="effectHandler">Effect handler to be used in execution.</param>
        /// <returns>A task computing the result of the eff computation.</returns>
        public async Task Run(IEffectHandler effectHandler)
        {
            var awaiter = GetAwaiterCore();
            await awaiter.Accept(effectHandler).ConfigureAwait(false);
            awaiter.GetResult();
        }
        
        // Helper method for exposing untyped variants of Run and GetAwaiter methods
        // Can be removed once Covariant return types are brought to C#.
        protected abstract EffAwaiter GetAwaiterCore();
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
        internal Eff()
        {

        }

        /// <summary>
        ///   Gets a new state machine instance for executing the eff computation.
        /// </summary>
        public abstract EffStateMachine<TResult> GetStateMachine();

        /// <summary>
        ///   Configures an EffAwaiter instance with supplied parameters.
        /// </summary>
        /// <param name="callerMemberName">The method or property name of the caller to the method.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number at the source file at which the method is called.</param>
        /// <returns>An EffAwaiter instance with callsite metadata.</returns>
        public new EffAwaiter<TResult> ConfigureAwait(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var awaiter = GetStateMachine();
            awaiter.CallerMemberName = callerMemberName;
            awaiter.CallerFilePath = callerFilePath;
            awaiter.CallerLineNumber = callerLineNumber;
            return awaiter;
        }

        /// <summary>
        ///   Implements the awaitable/awaiter pattern for Eff
        /// </summary>
        public new EffAwaiter<TResult> GetAwaiter() => GetStateMachine();

        /// <summary>
        ///   Executes the Eff computation using semantics from the provided effect handler.
        /// </summary>
        /// <param name="effectHandler">Effect handler to be used in execution.</param>
        /// <returns>A task computing the result of the eff computation.</returns>
        public new async Task<TResult> Run(IEffectHandler effectHandler)
        {
            var stateMachine = GetStateMachine();
            await effectHandler.Handle(stateMachine).ConfigureAwait(false);
            return stateMachine.GetResult();
        }

        protected sealed override EffAwaiter GetAwaiterCore() => GetStateMachine();

        public static implicit operator Eff<TResult>(Effect<TResult> effect) => Eff.FromEffect(effect);
    }
}
