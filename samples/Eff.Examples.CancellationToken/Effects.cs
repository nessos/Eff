using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Examples.CancellationToken
{

    public static class Effect
    {
        public static CancellationTokenEffect CancellationToken() => new CancellationTokenEffect();
    }
}
