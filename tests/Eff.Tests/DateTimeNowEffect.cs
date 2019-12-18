using System;

namespace Nessos.Eff.Tests
{
    public class DateTimeNowEffect : Effect<DateTime>
    {
        public DateTimeNowEffect(string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base (memberName, sourceFilePath, sourceLineNumber)
        {

        }
    }
}