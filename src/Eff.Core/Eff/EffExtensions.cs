using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public static class EffExtensions
    {
        public static EffAwaiter<TResult> AsEffect<TResult>(this Task<TResult> task,
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            async ValueTask<TResult> Wrap() { return await task;  }
            return new TaskEffAwaiter<TResult>(Wrap())
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }

        public static EffAwaiter AsEffect(this Task task,
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            async ValueTask<Unit> Wrap() { await task; return Unit.Value; }
            return new TaskEffAwaiter<Unit>(Wrap())
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }

        public static EffAwaiter<TResult> AsEffect<TResult>(this ValueTask<TResult> task,
                                    [CallerMemberName] string callerMemberName = "",
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerLineNumber] int callerLineNumber = 0)
        {
            return new TaskEffAwaiter<TResult>(task)
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }
    }
}