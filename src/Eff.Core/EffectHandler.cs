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

        public void Handle(DateTimeNowEffect effect)
        {
            effect.SetResult(DateTime.Now);
        }

        public async Task HandleAsync<TResult>(TaskEffect<TResult> effect)
        {
            var result = await effect.Task;
            effect.SetResult(result);
        }
    }


}
