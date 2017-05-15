using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public abstract class EffectHandler : IEffectHandler
    {

        public struct ExceptionLog
        {
            public string CallerMemberName;
            public string CallerFilePath;
            public int CallerLineNumber;
            public Exception Exception;
        }

        public struct ResultLog<TResult>
        {
            public string CallerMemberName;
            public string CallerFilePath;
            public int CallerLineNumber;
            public TResult result;
        }


        public abstract ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect);
        public abstract ValueTask<ValueTuple> Log(ExceptionLog log);
        public abstract ValueTask<ValueTuple> Log<TResult>(ResultLog<TResult> log);

        public virtual async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
            return ValueTuple.Create();
        }

        public virtual async ValueTask<ValueTuple> Handle(TaskEffect effect)
        {
            await effect.Task;
            effect.SetResult(ValueTuple.Create());
            return ValueTuple.Create();
        }

        public virtual async ValueTask<ValueTuple> Handle<TResult>(EffTaskEffect<TResult> effect)
        {
            var result = await effect.EffTask;
            effect.SetResult(result);
            return ValueTuple.Create();
        }

    }


}
