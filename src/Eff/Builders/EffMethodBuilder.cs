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
        private EffStateMachine<Unit>? _stateMachine;

        public static EffMethodBuilder Create()
        {
            return new EffMethodBuilder();
        }

        public Eff? Task { get; private set; }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            Task = new StateMachineEff<EffMethodBuilder, TStateMachine, Unit>(in stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // hijacks the IAsyncStateMachine.SetStateMachine mechanism
            // in order to pass the eff state machine instance to the builder
            // only used when evaluating async methods from release builds.
            _stateMachine = (EffStateMachine<Unit>)stateMachine;
        }

        void IEffMethodBuilder<Unit>.SetStateMachine(EffStateMachine<Unit> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void SetResult()
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.BuilderSetResult(Unit.Value);
        }

        public void SetException(Exception exception)
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.BuilderSetException(exception);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.BuilderSetAwaiter(awaiter);
        }

        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_stateMachine != null);
            _stateMachine!.BuilderSetAwaiter(awaiter);
        }
    }
}
