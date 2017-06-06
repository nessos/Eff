using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    [AsyncMethodBuilder(typeof(EffMethodBuilder<>))]
    public abstract class Eff<TResult>
    {
    }

    public class Await<TResult> : Eff<TResult>
    {
        private readonly IEffect effect;
        private readonly Func<Eff<TResult>> continuation;
        private readonly IAsyncStateMachine stateMachine;

        public Await(IEffect effect, Func<Eff<TResult>> continuation, IAsyncStateMachine stateMachine)
        {
            this.effect = effect;
            this.continuation = continuation;
            this.stateMachine = stateMachine;
        }

        public IEffect Effect => effect;
        public Func<Eff<TResult>> Continuation => continuation;
        public IAsyncStateMachine StateMachine => stateMachine;

    }

    public class SetResult<TResult> : Eff<TResult>
    {
        private readonly TResult result;

        public SetResult(TResult result)
        {
            this.result = result;
        }

        public TResult Result => result;

    }

    public class SetException<TResult> : Eff<TResult>
    {
        private readonly Exception exception;

        public SetException(Exception exception)
        {
            this.exception = exception;
        }

        public Exception Exception => exception;

    }

    public class Delay<TResult> : Eff<TResult>
    {
        private readonly Func<Eff<TResult>> func;

        public Delay(Func<Eff<TResult>> func)
        {
            this.func = func;
        }

        public Func<Eff<TResult>> Func => func;
    }


}
