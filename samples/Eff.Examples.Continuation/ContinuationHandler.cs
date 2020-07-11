using Nessos.Effects.Handlers;
using System;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Continuation
{
    public static class ContinuationHandler
    {
        /// <summary>
        ///   Executes an Eff computation with a call/cc effect handler,
        ///   using provided success and exception continuations.
        /// </summary>
        public static async Task StartWithContinuations<T>(this Eff<T> eff, Func<T, Task> onSuccess, Func<Exception, Task>? onException = null)
        {
            var continuationHandler = new ContinuationHandler<T>(onSuccess, onException ?? (_ => Task.CompletedTask));
            var stateMachine = eff.GetStateMachine();
            await continuationHandler.Handle(stateMachine);
        }
    }

    public class ContinuationHandler<TRootResult> : IEffectHandler
    {
        private int _depth = 0;
        private bool _isContinuationCaptured = false;
        private readonly Func<TRootResult, Task> _onSuccess;
        private readonly Func<Exception, Task> _onException;

        public ContinuationHandler(Func<TRootResult, Task> onSuccess, Func<Exception, Task> onException)
        {
            _onSuccess = onSuccess;
            _onException = onException;
        }

        public async ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            switch (awaiter.Effect)
            {
                case CallCcEffect<TResult> callCC:
                    _isContinuationCaptured = true; // abandon execution on the current evaluation stack
                    await callCC.Body(r => OnSuccess(awaiter, r), e => OnException(awaiter, e));
                    break;
            }
        }

        public async ValueTask Handle<TResult>(EffStateMachine<TResult> stateMachine)
        {
            while (!_isContinuationCaptured)
            {
                stateMachine.MoveNext();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                        if (_depth == 0)
                        {
                            var rootStateMachine = (EffStateMachine<TRootResult>)(object)stateMachine;
                            await _onSuccess(rootStateMachine.Result);
                        }
                        return;

                    case StateMachinePosition.Exception:
                        if (_depth == 0)
                        {
                            await _onException(stateMachine.Exception!);
                        }
                        return;

                    case StateMachinePosition.TaskAwaiter:
                        await stateMachine.TaskAwaiter!.Value;
                        break;

                    case StateMachinePosition.EffAwaiter:
                        var awaiter = stateMachine.EffAwaiter!;
                        _depth++;
                        try
                        {
                            await awaiter.Accept(this);
                        }
                        catch (Exception ex)
                        {
                            awaiter.SetException(ex);
                        }
                        _depth--;
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
            var handler = new ContinuationHandler<TRootResult>(_onSuccess, _onException) { _depth = _depth };
            IEffStateMachine? effStateMachine = awaiter.AwaitingStateMachine;

            while (effStateMachine != null)
            {
                handler._depth--;
                await effStateMachine.Accept(handler);
                effStateMachine = effStateMachine.AwaitingStateMachine;
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