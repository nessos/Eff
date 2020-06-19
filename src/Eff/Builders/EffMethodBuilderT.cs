using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Typed Eff method builder
    /// </summary>
    public struct EffMethodBuilder<TResult> : IEffMethodBuilder<TResult>
    {
        private DelayEff<TResult>? _delayEff;
        private EffStateMachine<TResult>? _stateMachine;

        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public Eff<TResult>? Task => _delayEff;

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _delayEff = new DelayEff<EffMethodBuilder<TResult>, TStateMachine, TResult>(in stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _stateMachine = (EffStateMachine<TResult>)stateMachine;
        }

        void IEffMethodBuilder<TResult>.SetStateMachine(EffStateMachine<TResult> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void SetResult(TResult result)
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.SetEff(new ResultEff<TResult>(result, _stateMachine));
        }

        public void SetException(Exception exception)
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.SetEff(new ExceptionEff<TResult>(exception, _stateMachine));
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_stateMachine != null);
            awaiter.SetState(_stateMachine!);
            _stateMachine!.SetEff(new AwaitEff<TResult>(awaiter, _stateMachine));
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_stateMachine != null);
            awaiter.SetState(_stateMachine!);
            _stateMachine!.SetEff(new AwaitEff<TResult>(awaiter, _stateMachine!));
        }
    }
}
