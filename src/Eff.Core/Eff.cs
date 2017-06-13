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
        private readonly IAsyncStateMachine stateMachine;

        public SetResult(TResult result, IAsyncStateMachine stateMachine)
        {
            this.result = result;
            this.stateMachine = stateMachine;
        }

        public TResult Result => result;
        public IAsyncStateMachine StateMachine => stateMachine;

    }

    public class SetException<TResult> : Eff<TResult>
    {
        private readonly Exception exception;
        private readonly IAsyncStateMachine stateMachine;

        public SetException(Exception exception, IAsyncStateMachine stateMachine)
        {
            this.exception = exception;
            this.stateMachine = stateMachine;
        }

        public Exception Exception => exception;
        public IAsyncStateMachine StateMachine => stateMachine;

    }

    public class Delay<TResult> : Eff<TResult>
    {
        private readonly Func<Eff<TResult>> func;
        private readonly IAsyncStateMachine stateMachine;

        public Delay(Func<Eff<TResult>> func, IAsyncStateMachine stateMachine)
        {
            this.func = func;
            this.stateMachine = stateMachine;
        }

        public Func<Eff<TResult>> Func => func;
        public IAsyncStateMachine StateMachine => stateMachine;
    }


}
