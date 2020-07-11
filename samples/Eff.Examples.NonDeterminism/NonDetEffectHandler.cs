using Nessos.Effects.Handlers;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Nessos.Effects.Examples.NonDeterminism
{
    public static class NonDetEffectHandler
    {
        public static async Task<TResult[]> Run<TResult>(Eff<TResult> eff)
        {
            var stateMachine = eff.GetStateMachine();
            var handler = new NonDetEffectHandler<TResult>();
            await handler.Handle(stateMachine);
            return handler.ResultHolder.GetResults();
        }
    }

    public class NonDetEffectHandler<TResult> : IEffectHandler
    {
        private int _depth = 0;
        private bool _isContinuationCaptured = false;

        public NonDetEffectHandler(NonDetResultHolder? result = null)
        {
            ResultHolder = result ?? new NonDetResultHolder();
        }

        public NonDetResultHolder ResultHolder { get; }

        public async ValueTask Handle<TValue>(EffectAwaiter<TValue> awaiter)
        {
            switch (awaiter.Effect)
            {
                case NonDetEffect<TValue> nde:
                    _isContinuationCaptured = true; // abandon execution on the current evaluation stack

                    foreach (var result in nde.Choices)
                    {
                        var clonedAwaiter = CloneAwaiter(awaiter);
                        clonedAwaiter.SetResult(result);
                        await ExecuteAwaiterContinuation(clonedAwaiter);
                    }

                    break;
            }
        }

        public async ValueTask Handle<TValue>(EffStateMachine<TValue> stateMachine)
        {
            while (!_isContinuationCaptured)
            {
                stateMachine.MoveNext();

                switch (stateMachine.Position)
                {
                    case StateMachinePosition.Result:
                        if (_depth == 0)
                        {
                            var rootStateMachine = (EffStateMachine<TResult>)(object)stateMachine;
                            ResultHolder.Values.Add(rootStateMachine.Result);
                        }
                        return;

                    case StateMachinePosition.Exception:
                        if (_depth == 0)
                        {
                            ResultHolder.Exception = stateMachine.Exception;
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
                        catch (Exception e)
                        {
                            awaiter.SetException(e);
                        }
                        _depth--;

                        break;

                    default:
                        throw new Exception($"Invalid state machine position {stateMachine.Position}.");
                }
            }
        }

        // Executes the awaiter continuation using a fresh copy of the effect handler
        private async Task ExecuteAwaiterContinuation<TValue>(EffAwaiter<TValue> awaiter)
        {
            if (ResultHolder.Exception != null)
            {
                // another nondeterministic branch has completed with an exception, yield.
                return;
            }

            var handler = new NonDetEffectHandler<TResult>(ResultHolder) { _depth = _depth };
            IEffStateMachine? effStateMachine = awaiter.AwaitingStateMachine;

            while (effStateMachine != null)
            {
                handler._depth--;
                await effStateMachine.Accept(handler);
                effStateMachine = effStateMachine.AwaitingStateMachine;
            }
        }

        private static EffectAwaiter<TValue> CloneAwaiter<TValue>(EffectAwaiter<TValue> awaiter)
        {
            var newAwaiter = new EffectAwaiter<TValue>(awaiter.Effect);

            if (awaiter.AwaitingStateMachine is IEffStateMachine sm)
            {
                var clonedSm = sm.Clone();
                clonedSm.UnsafeSetAwaiter(newAwaiter);
                newAwaiter.AwaitingStateMachine = clonedSm;
            }

            return newAwaiter;
        }

        public class NonDetResultHolder
        {
            public List<TResult> Values { get; } = new List<TResult>();
            public Exception? Exception { get; set; }
            public TResult[] GetResults()
            {
                if (Exception is Exception e)
                {
                    ExceptionDispatchInfo.Capture(e).Throw();
                }

                return Values.ToArray();
            }
        }
    }
}
