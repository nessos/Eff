using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class DateTimeNowEffect : Effect<DateTime>
    {
        
        
        public DateTimeNowEffect(string memberName, string sourceFilePath, int sourceLineNumber)
            : base (memberName, sourceFilePath, sourceLineNumber)
        {
        }

        public override void Accept(IEffectHandler handler)
        {
            handler.Handle(this);
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
