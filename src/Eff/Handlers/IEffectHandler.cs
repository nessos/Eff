using System.Threading.Tasks;
using Nessos.Effects.Builders;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Abstract effect handler. Implementations provide interpretation semantics for Eff computations.
    /// </summary>
    public interface IEffectHandler
    {
        /// <summary>
        ///   Handles abstract effect awaiters
        /// </summary>
        Task Handle<TResult>(EffectAwaiter<TResult> awaiter);

        /// <summary>
        ///   Handles nested Eff computation awaiters
        /// </summary>
        Task Handle<TResult>(EffAwaiter<TResult> awaiter);

        /// <summary>
        ///   Handles TPL task awaiters
        /// </summary>
        Task Handle<TResult>(TaskAwaiter<TResult> awaiter);

        //--------------------------------------------------//

        /// <summary>
        ///   Handles an eff computation with a materialized result
        /// </summary>
        Task<TResult> Handle<TResult>(ResultEff<TResult> resultEff);

        /// <summary>
        ///   Handles an eff computation that has thrown an exception
        /// </summary>
        Task Handle<TResult>(ExceptionEff<TResult> exceptionEff);

        /// <summary>
        ///   Handles a delayed eff computation
        /// </summary>
        Task<Eff<TResult>> Handle<TResult>(DelayEff<TResult> delayEff);

        /// <summary>
        ///   Handles an awaiting eff computation
        /// </summary>
        Task<Eff<TResult>> Handle<TResult>(AwaitEff<TResult> awaitEff);
    }
}
