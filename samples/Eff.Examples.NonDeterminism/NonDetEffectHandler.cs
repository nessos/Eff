#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Examples.NonDeterminism
{
    public static class NonDetEffectHandler
    {
        public static Task<List<TResult>> Run<TResult>(Eff<TResult> eff) => Run(eff.GetEvaluator());

        private static async Task<List<TResult>> Run<TResult>(EffEvaluator<TResult> evaluator)
        {
            while (true)
            {
                evaluator.MoveNext();

                switch (evaluator.Position)
                {
                    case EffEvaluatorPosition.Result:
                        return new List<TResult> { evaluator.Result };
                    case EffEvaluatorPosition.Exception:
                        throw evaluator.Exception!;

                    case EffEvaluatorPosition.Await:
                        var awaiter = evaluator.Awaiter!;
                        var handler = new NonDetEffectHandlerImpl<TResult>(evaluator);
                        await awaiter.Accept(handler);
                        return handler.Results;

                    default:
                        throw new Exception($"Invalid evaluator state {evaluator.Position}.");
                }
            }
        }

        private class NonDetEffectHandlerImpl<TResult> : IEffectHandler
        {
            private readonly EffEvaluator<TResult> _evaluator;

            public NonDetEffectHandlerImpl(EffEvaluator<TResult> evaluator)
            {
                _evaluator = evaluator;
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
                            await ExecuteEvaluator(useClonedEvaluator: true);
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
                        await ExecuteEvaluator(useClonedEvaluator: true);
                        awaiter.Clear();
                    }
                }
                else
                {
                    awaiter.SetException(error);
                    await ExecuteEvaluator();
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

                await ExecuteEvaluator();
            }

            public Task<TValue> Handle<TValue>(Eff<TValue> _)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   Executes the state machine to completion, using non-deterministic semantics,
            ///   appending any results to the handler state.
            /// </summary>
            private async Task ExecuteEvaluator(bool useClonedEvaluator = false)
            {
                var evaluator = useClonedEvaluator ? _evaluator.Clone() : _evaluator;
                var results = await NonDetEffectHandler.Run(evaluator);
                Results.AddRange(results);
            }
        }
    }
}
