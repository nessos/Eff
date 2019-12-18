using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nessos.Eff;

namespace Eff.Examples.DependencyInjection
{
    public class DependencyEffect<T> : Effect<T>
    {
        public DependencyEffect(string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
        }
    }

    public static class Effect
    {
        public static DependencyEffect<T> GetDependency<T>(
                                                    [CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0,
                                                    bool captureState = false)
        {
            return new DependencyEffect<T>(memberName, sourceFilePath, sourceLineNumber, captureState);
        }
    }
}
