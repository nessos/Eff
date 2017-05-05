using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TestEffectHandler : IEffectHandler
    {
        private readonly DateTime now;
        public TestEffectHandler(DateTime now)
        {
            this.now = now;
        }

        public void Handle(DateTimeNowEffect effect)
        {
            effect.SetResult(now);
        }

        public Task HandleAsync<TResult>(TaskEffect<TResult> effect)
        {
            throw new NotImplementedException();
        }
    }


}
