using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class DefaultEffectHandler : EffectHandler
    {
        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Log(ExceptionLog log)
        {
            return ValueTuple.Create();
        }

        public override async ValueTask<ValueTuple> Log(ResultLog log)
        {
            return ValueTuple.Create();
        }
    }


}
