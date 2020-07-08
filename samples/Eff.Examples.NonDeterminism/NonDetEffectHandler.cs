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
            return stateMachine.IsCompleted ?
                new[] { stateMachine.Result } :
                handler.Result.GetResults();
        }
    }

    public class NonDetEffectHandler<TResult> : IEffectHandler
    {
        private bool _isContinuationCaptured = false;

        public NonDetEffectHandler(NonDetResult? result = null)
        {
            Result = result ?? new NonDetResult();
        }

        public NonDetResult Result { get; }

        public async Task Handle<TValue>(EffectAwaiter<TValue> awaiter)
        {
            switch (awaiter.Effect)
            {
                case NonDetEffect<TValue> nde:
                    _isContinuationCaptured = true;

                    foreach (var result in nde.Choices)
                    {
                        var clonedAwaiter = CloneAwaiter(awaiter);
                        clonedAwaiter.SetResult(result);
                        await ExecuteAwaiter(clonedAwaiter);
                    }

                    break;
            }
        }

        public async Task Handle<TValue>(EffStateMachine<TValue> stateMachine)
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
                        catch (Exception e)
                        {
                            awaiter.SetException(e);
                        }

                        break;

                    default:
                        throw new Exception($"Invalid state machine position {stateMachine.Position}.");
                }
            }
        }

        // Executes the awaiter continuation using a fresh copy of the effect handler
        private async Task ExecuteAwaiter<TValue>(EffAwaiter<TValue> awaiter)
        {
            if (Result.Exception != null)
            {
                return;
            }

            var handler = new NonDetEffectHandler<TResult>(Result);
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

            var rootStateMachine = (EffStateMachine<TResult>)effStateMachine;
            if (rootStateMachine.Exception is Exception e)
            {
                Result.Exception = e;
            }
            else
            {
                Result.Values.Add(rootStateMachine.Result);
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

        public class NonDetResult
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
