using Eff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.RecordReplay
{
    class Program
    {

        static async Eff<(DateTime, int)> Foo<T>()
            where T : struct, IDateTimeNowEffect, IRandomEffect
        {
            var now = await default(T).DateTimeNow();
            var rnd = await default(T).Random();

            return (now, rnd);
        }

        static void Main(string[] args)
        {
            var handler = new RecordEffectHandler();
            var _ = Foo<EffectExample>().Run(handler).Result;
        }
    }
}
