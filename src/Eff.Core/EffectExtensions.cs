using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public static class EffectExtensions
    {
        public static TaskEffect<TResult> AsEffect<TResult>(this Task<TResult> task,
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TaskEffect<TResult>(task, memberName, sourceFilePath, sourceLineNumber);
        }

        public static TaskEffect<Unit> AsEffect(this Task task,
                                                        [CallerMemberName] string memberName = "",
                                                        [CallerFilePath] string sourceFilePath = "",
                                                        [CallerLineNumber] int sourceLineNumber = 0)
        {
            async Task<Unit> Wrap() { await task; return Unit.Value; }
            return new TaskEffect<Unit>(Wrap(), memberName, sourceFilePath, sourceLineNumber);
        }

        public static EffEffect<TResult> AsEffect<TResult>(this Eff<TResult> eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffEffect<TResult>(eff, memberName, sourceFilePath, sourceLineNumber);
        }

        public static EffEffect<Unit> AsEffect(this Eff eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffEffect<Unit>(eff.Ignore(), memberName, sourceFilePath, sourceLineNumber);
        }
    }
}

namespace Nessos.Eff.ImplicitAwaitables
{
    public static class ImplicitAwaitableExtensions
    {
        public static TaskEffect<TResult> GetAwaiter<TResult>(this Task<TResult> task) => task.AsEffect();
        public static TaskEffect<Unit> GetAwaiter<TResult>(this Task task) => task.AsEffect();
        public static EffEffect<TResult> GetAwaiter<TResult>(this Eff<TResult> eff) => eff.AsEffect();
        public static EffEffect<Unit> GetAwaiter(this Eff eff) => eff.AsEffect();
    }
}
