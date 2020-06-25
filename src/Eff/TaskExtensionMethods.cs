using Nessos.Effects.Handlers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Effects
{
    public static class TaskExtensionMethods
    {
        /// <summary>
        ///   Configures task instance as an Eff awaiter.
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
            return new Handlers.TaskAwaiter<TResult>(new ValueTask<TResult>(task))
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }

        /// <summary>
        ///   Configures task instance as an Eff awaiter.
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
            async ValueTask<Unit> Wrap() { await task.ConfigureAwait(false); return Unit.Value; }
            return new Handlers.TaskAwaiter<Unit>(Wrap())
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }

        /// <summary>
        ///   Configures task instance as an Eff awaiter.
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
            return new Handlers.TaskAwaiter<TResult>(task)
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }

        /// <summary>
        ///   Configures task instance as an Eff awaiter.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public static EffAwaiter AsEff(this ValueTask task,
                                    [CallerMemberName] string callerMemberName = "",
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerLineNumber] int callerLineNumber = 0)
        {
            async ValueTask<Unit> Wrap() { await task.ConfigureAwait(false); return Unit.Value; }
            return new Handlers.TaskAwaiter<Unit>(Wrap())
            {
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
        }
    }
}