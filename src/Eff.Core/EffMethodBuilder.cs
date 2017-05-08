using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public struct EffMethodBuilder<TResult>
    {
        private AsyncTaskMethodBuilder<TResult> methodBuilder;
        private TResult result;
        private bool haveResult;
        private bool useBuilder;

        public static EffMethodBuilder<TResult> Create() =>
            new EffMethodBuilder<TResult>()
            {
                methodBuilder = AsyncTaskMethodBuilder<TResult>.Create()
            };

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            methodBuilder.Start(ref stateMachine);
        }


        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            methodBuilder.SetStateMachine(stateMachine);
        }

        
        public void SetResult(TResult result)
        {
            if (useBuilder)
            {
                methodBuilder.SetResult(result);
            }
            else
            {
                this.result = result;
                haveResult = true;
            }
        }

        public void SetException(Exception exception)
        {
            methodBuilder.SetException(exception);
        }

        public Eff<TResult> Task
        {
            get
            {
                if (haveResult)
                {
                    return new Eff<TResult>(result);
                }
                else
                {
                    useBuilder = true;
                    return new Eff<TResult>(methodBuilder.Task);
                }
            }
        }


        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            useBuilder = true;

            var handler = EffectExecutionContext.Handler;
            if (handler == null)
                throw new InvalidOperationException("EffectExecutionContext handler is empty");

            switch (awaiter)
            {
                case IEffect effect:
                    var task = effect.Accept(handler);
                    if (task.IsCompleted)
                    {
                        stateMachine.MoveNext();
                    }
                    else
                    {
                        var _awaiter = task.GetAwaiter();
                        methodBuilder.AwaitOnCompleted(ref _awaiter, ref stateMachine);
                    }
                    break;
                default:
                    methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
                    break;
            }
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            useBuilder = true;

            var handler = EffectExecutionContext.Handler;
            if (handler == null)
                throw new InvalidOperationException("EffectExecutionContext handler is empty");
                
            switch (awaiter) 
            {
                case IEffect effect:
                    var task = effect.Accept(handler);
                    if (task.IsCompleted)
                    {
                        stateMachine.MoveNext();
                    }
                    else
                    {
                        var _awaiter = task.GetAwaiter();
                        methodBuilder.AwaitUnsafeOnCompleted(ref _awaiter, ref stateMachine);
                    }
                    break;
                default:
                    methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
                    break;
            }
        }
    }

}
