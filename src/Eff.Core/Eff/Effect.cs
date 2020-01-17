using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nessos.Eff
{
    public abstract class Effect : Effect<Unit>
    {

    }

    public abstract class Effect<TResult>
    {
        public EffAwaiter<TResult> GetAwaiter() => new EffectEffAwaiter<TResult>(this);

        public EffAwaiter<TResult> ConfigureAwait([CallerMemberName] string callerMemberName = "",
                                                  [CallerFilePath] string callerFilePath = "",
                                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            return new EffectEffAwaiter<TResult>(this)
            {
                CallerMemberName = callerMemberName,
                CallerLineNumber = callerLineNumber,
                CallerFilePath = callerFilePath
            };
        }
    }
}
