using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Continuation
{
    public static class ContinuationHandler
    {
        public static async Task StartWithContinuations<T>(this Eff<T> eff, Func<T, Task> onSuccess, Func<Exception, Task>? onException = null)
        {
            var continuationHandler = new ContinuationHandler<T>(onSuccess, onException ?? (_ => Task.CompletedTask));
            var stateMachine = eff.GetStateMachine();
            await continuationHandler.Handle(stateMachine);
        }
    }

    public class ContinuationHandler<TRootResult> : IEffectHandler
    {
        private bool _isContinuationCaptured = false;
        private readonly Func<TRootResult, Task> _onSuccess;
        private readonly Func<Exception, Task> _onException;

        public ContinuationHandler(Func<TRootResult, Task> onSuccess, Func<Exception, Task> onException)
        {
            _onSuccess = onSuccess;
            _onException = onException;
        }

        public async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter.Effect)
            {
                case CallCcEffect<TResult> callCC:
                    _isContinuationCaptured = true;
                    await callCC.Body(r => OnSuccess(awaiter, r), e => OnException(awaiter, e));
                    break;
            }
        }

        public async Task Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            while (!_isContinuationCaptured)
            {
                stateMachine.MoveNext();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                    case StateMachinePosition.Exception:
                        return;

                    case StateMachinePosition.TaskAwaiter:
                        await stateMachine.TaskAwaiter!.Value;
                        break;

                    case StateMachinePosition.EffAwaiter:
                        var awaiter = stateMachine.EffAwaiter!;
                        try
                        {
                            await awaiter.Accept(this);
                        }
                        catch (Exception ex)
                        {
                            awaiter.SetException(ex);
                        }
                        break;

                    default:
                        throw new Exception("Invalid state machine position.");
                }
            }
        }

        private async Task OnSuccess<TResult>(EffectAwaiter<TResult> awaiter, TResult result)
        {
            var clonedAwaiter = CloneAwaiter(awaiter);
            clonedAwaiter.SetResult(result);
            await ExecuteContinuation(clonedAwaiter);
        }

        private async Task OnException<TResult>(EffectAwaiter<TResult> awaiter, Exception exception)
        {
            var clonedAwaiter = CloneAwaiter(awaiter);
            clonedAwaiter.SetException(exception);
            await ExecuteContinuation(clonedAwaiter);
        }

        private async Task ExecuteContinuation<TResult>(EffectAwaiter<TResult> awaiter)
        {
            var handler = new ContinuationHandler<TRootResult>(_onSuccess, _onException);
            IEffStateMachine effStateMachine = awaiter.AwaitingStateMachine!;
            await effStateMachine.Accept(handler);

            while (effStateMachine.AwaitingStateMachine != null)
            {
                effStateMachine = effStateMachine.AwaitingStateMachine;
                await effStateMachine.Accept(handler);
            }

            if (handler._isContinuationCaptured)
            {
                return;
            }

            var rootStateMachine = (EffStateMachine<TRootResult>)effStateMachine;
            if (rootStateMachine.Exception is Exception e)
            {
                await _onException(e);
            }
            else
            {
                await _onSuccess(rootStateMachine.Result);
            }
        }

        private static EffectAwaiter<TResult> CloneAwaiter<TResult>(EffectAwaiter<TResult> awaiter)
        {
            var newAwaiter = new EffectAwaiter<TResult>(awaiter.Effect);

            if (awaiter.AwaitingStateMachine is IEffStateMachine sm)
            {
                var clonedSm = sm.Clone();
                clonedSm.UnsafeSetAwaiter(newAwaiter);
                newAwaiter.AwaitingStateMachine = clonedSm;
            }

            return newAwaiter;
        }
    }
}