using System;
using System.Runtime.CompilerServices;
using System.Security;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Typed Eff method builder
    /// </summary>
    public class EffMethodBuilder<TResult> : EffStateMachine<TResult>
    {
        public Eff<TResult>? Task => _currentEff;

        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public void SetStateMachine(IAsyncStateMachine _)
        {
            throw new NotSupportedException();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _asyncStateMachine = stateMachine;
            _cloner = StateMachineCloner<EffMethodBuilder<TResult>,TStateMachine>.Cloner;
            _currentEff = new DelayEff<TResult>(this);
        }

        public void SetResult(TResult result)
        {
            _currentEff = new ResultEff<TResult>(result, _asyncStateMachine!);
        }

        public void SetException(Exception exception)
        {
            _currentEff = new ExceptionEff<TResult>(exception, _asyncStateMachine!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.SetState(_asyncStateMachine!);
            _currentEff = new AwaitEff<TResult>(awaiter, this);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.SetState(_asyncStateMachine!);
            _currentEff = new AwaitEff<TResult>(awaiter, this);
        }
    }
}
