using System.Threading.Tasks;

namespace Nessos.Eff
{
    /// <summary>
    /// Abstract effect handler. Implementations provide interpretation semantics for Eff computations.
    /// </summary>
    public interface IEffectHandler 
    {
        /// <summary>
        /// Handles nested Eff computation awaiters
        /// </summary>
        Task Handle<TResult>(EffEffAwaiter<TResult> awaiter);

        /// <summary>
        /// Handles abstract effect awaiters
        /// </summary>
        Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter);

        /// <summary>
        /// Handles TPL task awaiters
        /// </summary>
        Task Handle<TResult>(TaskEffAwaiter<TResult> awaiter);


        //--------------------------------------------------//


        /// <summary>
        /// Handles an eff computation with a materialized result
        /// </summary>
        Task<TResult> Handle<TResult>(ResultEff<TResult> resultEff);

        /// <summary>
        /// Handles an eff computation that has thrown an exception
        /// </summary>
        Task Handle<TResult>(ExceptionEff<TResult> exceptionEff);

        /// <summary>
        /// Handles a delayed eff computation
        /// </summary>
        Task<Eff<TResult>> Handle<TResult>(DelayEff<TResult> delayEff);

        /// <summary>
        /// Handles an awaiting eff computation
        /// </summary>
        Task<Eff<TResult>> Handle<TResult>(AwaitEff<TResult> awaitEff);
    }
}
