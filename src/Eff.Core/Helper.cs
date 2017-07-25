using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class Effect
    {

        public static TaskEffect<TResult> AsEffect<TResult>(this Task<TResult> task,
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false)
        {
            return new TaskEffect<TResult>(task, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public static TaskEffect<ValueTuple> AsEffect(this Task task,
                                                        [CallerMemberName] string memberName = "",
                                                        [CallerFilePath] string sourceFilePath = "",
                                                        [CallerLineNumber] int sourceLineNumber = 0, 
                                                        bool captureState = false)
        {
            async Task<ValueTuple> Wrap() { await task; return ValueTuple.Create(); }
            return new TaskEffect<ValueTuple>(Wrap(), memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public static EffEffect<TResult> AsEffect<TResult>(this Eff<TResult> eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new EffEffect<TResult>(eff, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        
    }
}
