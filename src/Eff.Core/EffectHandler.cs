using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffectHandler : IEffectHandler
    {

        public async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DateTimeNowEffect dateTimeNowEffect:
                    dateTimeNowEffect.SetResult(DateTime.Now);
                    break;
            }
            return ValueTuple.Create();
        }


        public async ValueTask<ValueTuple> Handle<TResult>(TaskEffect<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
            return ValueTuple.Create();
        }

        public async ValueTask<ValueTuple> Handle(TaskEffect effect)
        {
            await effect.Task;
            effect.SetResult(ValueTuple.Create());
            return ValueTuple.Create();
        }

        public async ValueTask<ValueTuple> Handle<TResult>(EffTaskEffect<TResult> effect)
        {
            var result = await effect.EffTask;
            effect.SetResult(result);
            return ValueTuple.Create();
        }
    }


}
