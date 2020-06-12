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

        /// <summary>
        ///   Handles a top-level eff computation
        /// </summary>
        Task<TResult> Handle<TResult>(Eff<TResult> eff);
    }
}
