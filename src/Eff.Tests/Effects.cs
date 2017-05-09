using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Tests
{
    public static class Effect
    {
        public static DateTimeNowEffect DateTimeNow([CallerMemberName] string memberName = "",
                                                    [CallerFilePath] string sourceFilePath = "",
                                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            return new DateTimeNowEffect(memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
