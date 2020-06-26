using Nessos.Effects.Handlers;
using System.Threading;
using System.Threading.Tasks;

namespace Nessos.Effects.Cancellation
{
    /// <summary>
    ///   Defines an effect handler that automatically cancels Eff workflows based on a provided cancellation token.
    /// </summary>
    public class CancellationEffectHandler : EffectHandler
    {
        protected CancellationToken Token { get; }

        /// <summary>
        ///   Creates a cancellation effect handler using provided cancellation token.
        /// </summary>
        /// <param name="token"></param>
        public CancellationEffectHandler(CancellationToken token)
        {
            Token = token;
        }

        public override Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            Token.ThrowIfCancellationRequested();

            switch (awaiter)
            {
                case EffectAwaiter<CancellationToken> { Effect: CancellationTokenEffect.Effect _ } awter :
                    awter.SetResult(Token);
                    break;
            };

            return Task.CompletedTask;
        }

        public override Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            Token.ThrowIfCancellationRequested();
            return base.Handle(stateMachine);
        }
    }
}
