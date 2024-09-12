namespace Nessos.Effects.Examples.Continuation;

using System;
using System.Threading;
using System.Threading.Tasks;

public static class Async
{
    // F#-style Async.Parallel combinator using call/cc
    public static Effect<T[]> Parallel<T>(params Eff<T>[] children)
    {
        return Effects.CallCC<T[]>((sk, ek) =>
        {
            var results = new T[children.Length];
            int completedTasks = 0;
            int isFaulted = 0;

            for (int i = 0; i < children.Length; i++)
            {
                _ = children[i].StartWithContinuations(
                    onSuccess: value => OnSuccess(i, value),
                    onException: OnException);
            }

            return Task.CompletedTask;

            async Task OnSuccess(int i, T value)
            {
                results![i] = value;
                if (Interlocked.Increment(ref completedTasks) == results.Length)
                {
                    await sk(results);
                }
            }

            async Task OnException(Exception e)
            {
                if (Interlocked.CompareExchange(ref isFaulted, 1, 0) == 0)
                {
                    await ek(e);
                }
            }
        });
    }
}
