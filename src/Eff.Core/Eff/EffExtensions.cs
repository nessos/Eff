using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public static class EffExtensions
    {
        /// <summary>
        /// Configures task instance as an Eff awaiter.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public static EffAwaiter<TResult> AsEff<TResult>(this Task<TResult> task,
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

        /// <summary>
        /// Configures task instance as an Eff awaiter.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public static EffAwaiter AsEff(this Task task,
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

        /// <summary>
        /// Configures task instance as an Eff awaiter.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public static EffAwaiter<TResult> AsEff<TResult>(this ValueTask<TResult> task,
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