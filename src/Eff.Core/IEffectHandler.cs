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
        bool EnableExceptionLogging { get; }
        bool EnableTraceLogging { get; }
        bool EnableParametersLogging { get; }
        bool EnableLocalVariablesLogging { get; }

        ValueTask<ValueTuple> Log(ExceptionLog log);
        ValueTask<ValueTuple> Log(ResultLog log);

        ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect);
        ValueTask<ValueTuple> Handle(TaskEffect effect);
        ValueTask<ValueTuple> Handle<TResult>(EffTaskEffect<TResult> effect);

        
    }

    
}
