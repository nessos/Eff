using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Eff.Core
{
    public struct EffTaskMethodBuilder<TResult>
    {
        private AsyncTaskMethodBuilder<TResult> methodBuilder;
        private TResult result;
        private bool haveResult;
        private bool useBuilder;
        private IEffectHandler handler;

        public static EffTaskMethodBuilder<TResult> Create()
        {
            var handler = EffectExecutionContext.Handler;
            if (handler == null)
                throw new EffException("EffectExecutionContext handler is empty");

            return new EffTaskMethodBuilder<TResult>()
            {
                handler = handler,
                methodBuilder = AsyncTaskMethodBuilder<TResult>.Create()
            };
        }

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

        public EffTask<TResult> Task
        {
            get
            {
                if (haveResult)
                {
                    return new EffTask<TResult>(result);
                }
                else
                {
                    useBuilder = true;
                    return new EffTask<TResult>(methodBuilder.Task);
                }
            }
        }


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
            useBuilder = true;

            switch (awaiter)
            {
                case IEffect effect:
                    async ValueTask<ValueTuple> ApplyEffectHandler(IEffectHandler _handler)
                    {
                        try
                        {
                            await effect.Accept(_handler);
                            if (!effect.IsCompleted)
                                throw new EffException($"Effect {effect.GetType().Name} is not completed.");
                            return ValueTuple.Create();
                        }
                        catch (AggregateException ex)
                        {
                            effect.SetException(ex.InnerException);
                            return ValueTuple.Create();
                        }
                        catch (Exception ex)
                        {
                            effect.SetException(ex);
                            return ValueTuple.Create();
                        }
                        finally
                        {
                        }
                    }
                    var task = ApplyEffectHandler(handler);
                    if (task.IsCompleted)
                    {
                        stateMachine.MoveNext();
                    }
                    else
                    {
                        var _awaiter = task.GetAwaiter();
                        if (safe)
                            methodBuilder.AwaitOnCompleted(ref _awaiter, ref stateMachine);
                        else
                            methodBuilder.AwaitUnsafeOnCompleted(ref _awaiter, ref stateMachine);
                    }
                    break;
                default:
                    throw new EffException($"Awaiter {awaiter.GetType().Name} is not an effect. Try to use obj.AsEffect().");
            }
        }


    }

}
