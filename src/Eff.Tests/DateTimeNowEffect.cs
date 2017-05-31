using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Tests
{
    public class DateTimeNowEffect : Effect<DateTime>
    {

        public DateTimeNowEffect(string memberName, string sourceFilePath, int sourceLineNumber, bool captureState)
            : base (memberName, sourceFilePath, sourceLineNumber, captureState)
        {
        }

        public override ValueTask<ValueTuple> Accept(IEffectHandler handler)
        {
            return handler.Handle(this);
        }

        public override void OnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }

        public override void UnsafeOnCompleted(Action continuation)
        {
            throw new NotSupportedException();
        }
    }

    
    
}
