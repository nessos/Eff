using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    public interface IDateTimeNowEffect
    {
        DateTimeNowEffect DateTimeNow([CallerMemberName] string memberName = "",
                                      [CallerFilePath] string sourceFilePath = "",
                                      [CallerLineNumber] int sourceLineNumber = 0);
    }

    public interface IRandomEffect
    {
        RandomEffect Random([CallerMemberName] string memberName = "",
                                 [CallerFilePath] string sourceFilePath = "",
                                 [CallerLineNumber] int sourceLineNumber = 0);
    }

    public struct EffectExample : IDateTimeNowEffect, IRandomEffect
    {

        public DateTimeNowEffect DateTimeNow([CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new DateTimeNowEffect(memberName, sourceFilePath, sourceLineNumber);
        }

        public RandomEffect Random([CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new RandomEffect(memberName, sourceFilePath, sourceLineNumber);
        }
    }

}
