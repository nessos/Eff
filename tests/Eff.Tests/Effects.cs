using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Tests
{
    public interface IDateTimeNowEffect
    {
        DateTimeNowEffect DateTimeNow([CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false);
    }
    public interface IFuncEffect
    {
        FuncEffect<TResult> Func<TResult>(Func<TResult> func,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false);
        FuncEffect<ValueTuple> Action(Action action,
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false);
    }
    public struct CustomEffect : IDateTimeNowEffect, IFuncEffect
    {
        public DateTimeNowEffect DateTimeNow([CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false)
        {
            return new DateTimeNowEffect(memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public FuncEffect<TResult> Func<TResult>(Func<TResult> func,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0,
                                            bool captureState = false)
        {
            return new FuncEffect<TResult>(func, memberName, sourceFilePath, sourceLineNumber, captureState);
        }

        public FuncEffect<ValueTuple> Action(Action action,
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false)
        {
            return new FuncEffect<ValueTuple>(() => { action(); return ValueTuple.Create(); }, memberName, sourceFilePath, sourceLineNumber, captureState);
        }
    }
}
