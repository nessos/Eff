using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffectHandler 
    {
        ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect);
        ValueTask<ValueTuple> Handle(TaskEffect effect);
    }

    
}
