using Nessos.Effects.Handlers;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Typed Eff method builder
    /// </summary>
    public struct EffMethodBuilder<TResult> : IEffMethodBuilder<TResult>
    {
        private EffStateMachine<TResult>? _effStateMachine;

        public static EffMethodBuilder<TResult> Create() => default;

        public Eff<TResult>? Task { get; private set; }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(TResult result)
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetResult(result);
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
