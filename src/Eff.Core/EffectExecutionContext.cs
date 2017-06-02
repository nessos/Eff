using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public static class EffectExecutionContext
    {
        [ThreadStatic]
        public static IEffMethodHandler Handler;
        
    }


}
