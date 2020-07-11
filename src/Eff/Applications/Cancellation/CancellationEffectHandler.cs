using Nessos.Effects.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace Nessos.Effects.Cancellation
{
    /// <summary>
    ///   Defines an effect handler that automatically cancels Eff workflows based on the provided cancellation token.
    /// </summary>
    public class CancellationEffectHandler : EffectHandler
    {
        /// <summary>
        ///   The cancellation token flowing through the Eff workflow.
        /// </summary>
        protected CancellationToken CancellationToken { get; }

        /// <summary>
        ///   Creates a cancellation effect handler using provided cancellation token.
        /// </summary>
        /// <param name="token"></param>
        public CancellationEffectHandler(CancellationToken token)
        {
            CancellationToken = token;
        }

        public override ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            CancellationToken.ThrowIfCancellationRequested();

            switch (awaiter)
            {
                case EffectAwaiter<CancellationToken> { Effect: CancellationTokenEffect _ } awter :
                    awter.SetResult(CancellationToken);
                    break;
            };

            return default;
        }

        public override ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            CancellationToken.ThrowIfCancellationRequested();
            return base.Handle(stateMachine);
        }
    }
}
