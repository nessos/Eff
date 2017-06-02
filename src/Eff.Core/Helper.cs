﻿using System;
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

        public static TaskEffect AsEffect(this Task task,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0, 
                                            bool captureState = false)
        {
            return new TaskEffect(task, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public static EffTaskEffect<TResult> AsEffect<TResult>(this Eff<TResult> eff,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new EffTaskEffect<TResult>(eff, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public static FuncEffect<TResult> Func<TResult>(Func<TResult> func,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new FuncEffect<TResult>(func, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public static ActionEffect Action(Action action,
                                    [CallerMemberName] string memberName = "",
                                    [CallerFilePath] string sourceFilePath = "",
                                    [CallerLineNumber] int sourceLineNumber = 0,
                                    bool captureState = false)
        {
            return new ActionEffect(action, memberName, sourceFilePath, sourceLineNumber, captureState);
        }
    }
}