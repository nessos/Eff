using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    public class DateTimeNowEffect : Effect<DateTime>
    {

        public DateTimeNowEffect(string memberName, string sourceFilePath, int sourceLineNumber)
            : base (memberName, sourceFilePath, sourceLineNumber)
        {
        }
    }

    public class RandomEffect : Effect<int>
    {

        public RandomEffect(string memberName, string sourceFilePath, int sourceLineNumber)
            : base(memberName, sourceFilePath, sourceLineNumber)
        {
        }
    }

    

}
