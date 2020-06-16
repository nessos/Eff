#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Examples.NonDeterminism
{
    public static class NonDetEffectHandler
    {
        public static async Task<List<TResult>> Run<TResult>(Eff<TResult> eff)
        {
            while (true)
            {
                switch (eff)
                {
                    case ResultEff<TResult> setResult:
                        return new List<TResult> { setResult.Result };
                    case ExceptionEff<TResult> setException:
                        throw setException.Exception;
                    case DelayEff<TResult> delay:
                        eff = delay.StateMachine.MoveNext();
                        break;

                    case AwaitEff<TResult> awaitEff:
                        var handler = new NonDetEffectHandler<TResult>(awaitEff.StateMachine);
                        await awaitEff.Awaiter.Accept(handler);
                        return handler.Results;

                    default:
                        throw new NotSupportedException($"{eff.GetType().Name}");
                }
            }
        }
    }

    public class NonDetEffectHandler<TResult> : IEffectHandler
    {
        private readonly EffStateMachine<TResult> _stateMachine;

        public NonDetEffectHandler(EffStateMachine<TResult> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public List<TResult> Results { get; } = new List<TResult>();

        public async Task Handle<TValue>(EffectAwaiter<TValue> awaiter)
        {
            switch (awaiter.Effect)
            {
                case NonDetEffect<TValue> nde:
                    foreach (var result in nde.Choices)
                    {
                        awaiter.SetResult(result);
                        await ExecuteStateMachine(useClonedStateMachine: true);
                        awaiter.Clear();
                    }

                    break;
            }
        }

        public async Task Handle<TValue>(EffAwaiter<TValue> awaiter)
        {
            List<TValue>? values = null;
            Exception? error = null;
            try
            {
                values = await NonDetEffectHandler.Run(awaiter.Eff);
            }
            catch (Exception e)
            {
                error = e;
            }

            if (error is null)
            {
                foreach (var result in values!)
                {
                    awaiter.SetResult(result);
                    await ExecuteStateMachine(useClonedStateMachine: true);
                    awaiter.Clear();
                }
            }
            else
            {
                awaiter.SetException(error);
                await ExecuteStateMachine();
            }
        }

        public async Task Handle<TValue>(TaskAwaiter<TValue> awaiter)
        {
            try
            {
                var result = await awaiter.Task;
                awaiter.SetResult(result);
            }
            catch (Exception e)
            {
                awaiter.SetException(e);
            }

            await ExecuteStateMachine();
        }

        public Task<TValue> Handle<TValue>(Eff<TValue> _)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Executes the state machine to completion, using non-deterministic semantics,
        ///   appending any results to the handler state.
        /// </summary>
        private async Task ExecuteStateMachine(bool useClonedStateMachine = false)
        {
            var stateMachine = useClonedStateMachine ? _stateMachine.Clone() : _stateMachine;
            var next = stateMachine.MoveNext();
            var results = await NonDetEffectHandler.Run(next);
            Results.AddRange(results);
        }
    }
}
