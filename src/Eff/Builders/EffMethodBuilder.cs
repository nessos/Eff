using Nessos.Effects.Handlers;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Untyped Eff method builder
    /// </summary>
    public struct EffMethodBuilder : IEffMethodBuilder<Unit>
    {
        private EffStateMachine<Unit>? _effStateMachine;

        public static EffMethodBuilder Create() => default;

        public Eff? Task { get; private set; }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            Task = new StateMachineEff<EffMethodBuilder, TStateMachine, Unit>(in stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // hijacks the IAsyncStateMachine.SetStateMachine mechanism
            // in order to pass the eff state machine instance to the builder
            // only used when evaluating async methods from release builds.
            _effStateMachine = (EffStateMachine<Unit>)stateMachine;
        }

        void IEffMethodBuilder<Unit>.SetStateMachine(EffStateMachine<Unit> stateMachine)
        {
            _effStateMachine = stateMachine;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult()
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetResult(Unit.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception exception)
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetException(exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetAwaiter(ref awaiter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.UnsafeBuilderSetAwaiter(ref awaiter);
        }
    }
}
