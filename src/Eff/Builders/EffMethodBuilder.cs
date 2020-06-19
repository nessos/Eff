using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Untyped Eff method builder
    /// </summary>
    public struct EffMethodBuilder : IEffMethodBuilder<Unit>
    {
        private DelayEff<Unit>? _delayEff;
        private EffStateMachine<Unit>? _stateMachine;

        public static EffMethodBuilder Create()
        {
            return new EffMethodBuilder();
        }

        public Eff? Task => _delayEff;

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _delayEff = new DelayEff<EffMethodBuilder, TStateMachine, Unit>(in stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _stateMachine = (EffStateMachine<Unit>)stateMachine;
        }

        void IEffMethodBuilder<Unit>.SetStateMachine(EffStateMachine<Unit> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void SetResult()
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.SetEff(new ResultEff<Unit>(Unit.Value, _stateMachine));
        }

        public void SetException(Exception exception)
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.SetEff(new ExceptionEff<Unit>(exception, _stateMachine));
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_stateMachine != null);
            awaiter.SetState(_stateMachine!);
            _stateMachine!.SetEff(new AwaitEff<Unit>(awaiter, _stateMachine));
        }

        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_stateMachine != null);
            awaiter.SetState(_stateMachine!);
            _stateMachine!.SetEff(new AwaitEff<Unit>(awaiter, _stateMachine));
        }
    }
}
