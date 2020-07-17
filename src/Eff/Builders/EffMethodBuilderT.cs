using Nessos.Effects.Handlers;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Typed Eff method builder.
    /// </summary>
    public struct EffMethodBuilder<TResult> : IEffMethodBuilder<TResult>
    {
        private EffStateMachine<TResult>? _effStateMachine;

        /// <summary>
        ///   Creates a new method builder. For use by the generated state machine.
        /// </summary>
        public static EffMethodBuilder<TResult> Create() => default;

        /// <summary>
        ///   Returns the built eff instance. For use by the generated state machine.
        /// </summary>
        public Eff<TResult>? Task { get; private set; }

        /// <summary>
        ///   Passes the async state machine to the method builder. For use by the generated state machine.
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            Task = new StateMachineEff<EffMethodBuilder<TResult>, TStateMachine, TResult>(in stateMachine);
        }

        /// <summary>
        ///   Sets a Eff state machine instance. For use by the generated state machine.
        /// </summary>
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

        /// <summary>
        ///   Sets the result of the computation. For use by the generated state machine.
        /// </summary>
        public void SetResult(TResult result)
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetResult(result);
        }

        /// <summary>
        ///   Sets an exception for the computation. For use by the generated state machine.
        /// </summary>
        public void SetException(Exception exception)
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetException(exception);
        }

        /// <summary>
        ///   Sets an awaiter instance. For use by the generated state machine.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_effStateMachine != null);
            _effStateMachine!.BuilderSetAwaiter(ref awaiter);
        }

        /// <summary>
        ///   Sets an awaiter instance. For use by the generated state machine.
        /// </summary>
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
