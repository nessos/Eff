using System.Threading.Tasks;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   An abstraction for providing evaluation semantics for Eff computations.
    /// </summary>
    public interface IEffectHandler
    {

        /// <summary>
        ///    Provides handling logic for abstract Effect awaiters.
        /// </summary>
        /// <typeparam name="TResult">The result type of the abstract effect.</typeparam>
        /// <param name="awaiter">The effect awaiter to be completed with a value.</param>
        /// <returns>A task waiting on the asynchronous handler.</returns>
        /// <remarks>
        ///   Typically a pattern match against subtypes of <see cref="Effect{TResult}"/>
        ///   that are recognized by the particular effect handler implementation.
        /// </remarks>
        ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter);

        /// <summary>
        ///   Provides evaluation logic for Eff state machines.
        /// </summary>
        /// <typeparam name="TResult">The result type of the eff state machine.</typeparam>
        /// <param name="stateMachine">The state machine to be evaluated.</param>
        /// <returns>A task waiting on the asynchronous evaluation.</returns>
        /// <remarks>
        ///   Typically defines an evaluation loop calling the <see cref="EffStateMachine{TResult}.MoveNext"/> method
        ///   and querying the <see cref="EffStateMachine{TResult}.Position"/> property.
        ///   On completion the state machine should be completed with either a result or exception.
        /// </remarks>
        ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine);
    }
}
