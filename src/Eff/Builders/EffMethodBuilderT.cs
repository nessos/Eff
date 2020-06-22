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
        private EffStateMachine<TResult>? _effStateMachine;

        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public Eff<TResult>? Task { get; private set; }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            Task = new StateMachineEff<EffMethodBuilder<TResult>, TStateMachine, TResult>(in stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _effStateMachine = (EffStateMachine<TResult>)stateMachine;
        }

        void IEffMethodBuilder<TResult>.SetStateMachine(EffStateMachine<TResult> stateMachine)
        {
            // hijacks the IAsyncStateMachine.SetStateMachine mechanism,
            // in order to pass the state machine instance to the builder.
            // Only used when evaluating async methods from release builds.
            _effStateMachine = stateMachine;
        }

        public void SetResult(TResult result)
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetResult(result);
        }

        public void SetException(Exception exception)
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetException(exception);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetAwaiter(awaiter);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetAwaiter(awaiter);
        }
    }
}
