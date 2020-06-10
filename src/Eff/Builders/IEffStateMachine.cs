namespace Nessos.Effects.Builders
{
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
