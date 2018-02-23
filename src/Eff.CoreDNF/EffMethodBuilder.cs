using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public class EffMethodBuilder<TResult>
    {
        private Eff<TResult> eff;
        private object state;
        private Func<object, Eff<TResult>> continuation;

        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            this.state = stateMachine;
            this.continuation = state =>
            {
                this.state = state;
                ((IAsyncStateMachine)state).MoveNext();
                return this.eff;
            };
            this.eff = new Delay<TResult>(continuation, state);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            
        }

        public void SetResult(TResult result)
        {
            this.eff = new SetResult<TResult>(result, state);
        }

        public void SetException(Exception exception)
        {
            this.eff = new SetException<TResult>(exception, state);
        }

        public Eff<TResult> Task => this.eff;
        
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine, true);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine, false);
        }

        private void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine, bool safe)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {

            switch (awaiter)
            {
                case IEffect effect:
                    effect.SetState(state);
                    this.eff = new Await<TResult>(effect, continuation, state);

                    break;
                default:
                    throw new EffException($"Awaiter {awaiter.GetType().Name} is not an effect. Try to use obj.AsEffect().");
            }
        }


    }

}
