using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class Effect
    {
        public static TaskEffect<TResult> AsEffect<TResult>(this Task<TResult> task,
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new TaskEffect<TResult>(task, memberName, sourceFilePath, sourceLineNumber);
        }

        public static TaskEffect<ValueTuple> AsEffect(this Task task,
                                                        [CallerMemberName] string memberName = "",
                                                        [CallerFilePath] string sourceFilePath = "",
                                                        [CallerLineNumber] int sourceLineNumber = 0)
        {
            async Task<ValueTuple> Wrap() { await task; return ValueTuple.Create(); }
            return new TaskEffect<ValueTuple>(Wrap(), memberName, sourceFilePath, sourceLineNumber);
        }

        public static EffEffect<TResult> AsEffect<TResult>(this Eff<TResult> eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new EffEffect<TResult>(eff, memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
