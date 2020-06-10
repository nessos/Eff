using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nessos.Effects.Builders;
using Nessos.Effects.Handlers;

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
        public static EffAwaiterBase<TResult> AsEff<TResult>(this Task<TResult> task,
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            async ValueTask<TResult> Wrap() { return await task;  }
            return new Handlers.TaskAwaiter<TResult>(Wrap())
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
        public static EffAwaiterBase AsEff(this Task task,
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            async ValueTask<Unit> Wrap() { await task; return Unit.Value; }
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
        public static EffAwaiterBase<TResult> AsEff<TResult>(this ValueTask<TResult> task,
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
    }
}