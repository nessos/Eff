using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Nessos.Effects.Builders;

namespace Nessos.Effects.Handlers
{
    /// <summary>
    ///   Provides a base effect handler implementation using regular async method semantics.
    /// </summary>
    public abstract class EffectHandler : IEffectHandler
    {
        public abstract Task Handle<TResult>(EffectAwaiter<TResult> awaiter);
       
        public virtual async Task Handle<TResult>(TaskAwaiter<TResult> awaiter)
        {
            var result = await awaiter.Task.ConfigureAwait(false);
            awaiter.SetResult(result);
        }

        public virtual async Task Handle<TResult>(EffAwaiter<TResult> awaiter)
        {
            var result = await Handle(awaiter.Eff).ConfigureAwait(false);
            awaiter.SetResult(result);
        }

        public virtual async Task<TResult> Handle<TResult>(Eff<TResult> eff)
        {
            if (eff is null)
            {
                throw new ArgumentNullException(nameof(eff));
            }

            var evaluator = eff.GetEvaluator();

            while (true)
            {
                evaluator.MoveNext();

                switch (evaluator.Position)
                {
                    case EffEvaluatorPosition.Result:
                        return evaluator.Result!;
                    case EffEvaluatorPosition.Exception:
                        ExceptionDispatchInfo.Capture(evaluator.Exception!).Throw();
                        return default!;
                    case EffEvaluatorPosition.Await:
                        var awaiter = evaluator.Awaiter!;
                        try
                        {
                            await awaiter.Accept(this).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            // clear any existing results and surface the current exception
                            if (awaiter.IsCompleted)
                            {
                                awaiter.Clear();
                            }

                            awaiter.SetException(ex);
                        }
                        break;

                    default:
                        Debug.Fail($"Unrecognized evaluator state {evaluator.Position}.");
                        throw new Exception($"Internal error: unrecognized evaluator state {evaluator.Position}.");
                }
            }
        }
    }
}
