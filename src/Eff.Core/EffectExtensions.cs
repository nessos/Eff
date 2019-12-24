using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public static class EffectExtensions
    {
        public static EffAwaiter<TResult> AsEffect<TResult>(this Eff<TResult> eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffAwaiter<TResult>(eff, memberName, sourceFilePath, sourceLineNumber);
        }

        public static EffAwaiter<Unit> AsEffect(this Eff eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffAwaiter<Unit>(eff.Ignore(), memberName, sourceFilePath, sourceLineNumber);
        }

        public static EffectAwaiter<TResult> AsEffect<TResult>(this Effect<TResult> effect,
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffectAwaiter<TResult>(effect, memberName, sourceFilePath, sourceLineNumber);
        }

        public static TaskAwaiter<TResult> AsEffect<TResult>(this Task<TResult> task,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TaskAwaiter<TResult>(task, memberName, sourceFilePath, sourceLineNumber);
        }

        public static TaskAwaiter<Unit> AsEffect(this Task task,
                                                        [CallerMemberName] string memberName = "",
                                                        [CallerFilePath] string sourceFilePath = "",
                                                        [CallerLineNumber] int sourceLineNumber = 0)
        {
            async Task<Unit> Wrap() { await task; return Unit.Value; }
            return new TaskAwaiter<Unit>(Wrap(), memberName, sourceFilePath, sourceLineNumber);
        }
    }
}

namespace Nessos.Eff.ImplicitAwaitables
{
    public static class ImplicitAwaitableExtensions
    {
        public static EffAwaiter<TResult> GetAwaiter<TResult>(this Eff<TResult> eff) => eff.AsEffect();
        public static EffAwaiter<Unit> GetAwaiter(this Eff eff) => eff.AsEffect();
        public static EffectAwaiter<TResult> GetAwaiter<TResult>(this Effect<TResult> effect) => effect.AsEffect();
    }
}
