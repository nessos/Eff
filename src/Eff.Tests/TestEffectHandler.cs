using Eff.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class TestEffectHandler : EffectHandler
    {
        private readonly DateTime now;
        public TestEffectHandler(DateTime now)
        {
            this.now = now;
        }

        public TestEffectHandler() : this(DateTime.Now)
        { }

        public override async ValueTask<ValueTuple> Handle<TResult>(IEffect<TResult> effect)
        {
            switch (effect)
            {
                case DateTimeNowEffect dateTimeNowEffect:
                    dateTimeNowEffect.SetResult(now);
                    break;
            }

            return ValueTuple.Create();
        }

        public override ValueTask<ValueTuple> Log(ExceptionLog log)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<ValueTuple> Log<TResult>(ResultLog<TResult> log)
        {
            throw new NotImplementedException();
        }
    }


}
