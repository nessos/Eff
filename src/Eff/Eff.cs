using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Effects
{
    /// <summary>
    ///   Exposes an effectful computation that can be executed using an <see cref="IEffectHandler"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Execution of the computation is delayed ("cold semantics").
    /// </para>
    /// <para>
    ///   To start an <see cref="Eff"/> computation, an effect handler must by supplied via the <see cref="Run"/> method.
    /// </para>
    /// </remarks>
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
        ///   Gets an <see cref="EffAwaiter" /> instance containing this Eff.
        /// </summary>
        public EffAwaiter GetAwaiter() => GetAwaiterCore();

        /// <summary>
        ///    Executes the Eff computation using semantics from the provided effect handler.
        /// </summary>
        /// <param name="effectHandler">Effect handler to be used in execution.</param>
        /// <returns>A task waiting on the result of the eff computation.</returns>
        /// <remarks>
        ///   Each <see cref="Run"/> operation will execute a fresh copy of the underlying async state machine.
        ///   A single Eff instance can be safely executed multiple times.
        /// </remarks>
        public async ValueTask Run(IEffectHandler effectHandler)
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
    ///   Exposes an effectful computation that can be executed using an <see cref="IEffectHandler"/>.
    /// </summary>
    /// <typeparam name="TResult">The return type of the computation.</typeparam>
    /// <remarks>
    /// <para>
    ///   Execution of the computation is delayed ("cold semantics").
    /// </para>
    /// <para>
    ///   To start an <see cref="Eff"/> computation, an effect handler must by supplied via the <see cref="Run"/> method.
    /// </para>
    /// </remarks>
    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult> : Eff
    {
        internal Eff()
        {

        }

        /// <summary>
        ///   Gets a new state machine instance for executing the eff computation.
        /// </summary>
        /// <remarks>
        ///   Each call will return a shallow copy of the underlying async state machine.
        ///   These can be run independently, with semantics identical to invoking delegates.
        /// </remarks>
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
        ///   Gets an <see cref="EffAwaiter{TResult}" /> instance containing this Eff.
        /// </summary>
        public new EffAwaiter<TResult> GetAwaiter() => GetStateMachine();

        /// <summary>
        ///   Executes the Eff computation using semantics from the provided effect handler.
        /// </summary>
        /// <param name="effectHandler">Effect handler to be used in execution.</param>
        /// <returns>A task waiting on the result of the eff computation.</returns>
        /// <remarks>
        ///   Each <see cref="Run"/> operation will execute a fresh copy of the underlying async state machine.
        ///   A single Eff instance can be safely executed multiple times.
        /// </remarks>
        public new async ValueTask<TResult> Run(IEffectHandler effectHandler)
        {
            var stateMachine = GetStateMachine();
            await effectHandler.Handle(stateMachine).ConfigureAwait(false);
            return stateMachine.GetResult();
        }

        public static implicit operator Eff<TResult>(Effect<TResult> effect) => Eff.FromEffect(effect);

        protected sealed override EffAwaiter GetAwaiterCore() => GetStateMachine();
    }
}
