using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public abstract class Effect
    {
        internal Effect() { }
    }

    public abstract class Effect<TResult> : Effect
    {

    }
}
