using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    public class ReplayEffectHandler : EffectHandler
    {
        public override ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            throw new NotImplementedException();
        }
    }
}
