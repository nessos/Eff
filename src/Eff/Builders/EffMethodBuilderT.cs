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
        private EffEvaluator<TResult>? _evaluator;

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
            _evaluator = (EffEvaluator<TResult>)stateMachine;
        }

        void IEffMethodBuilder<TResult>.SetEvaluator(EffEvaluator<TResult> evaluator)
        {
            // hijacks the IAsyncStateMachine.SetStateMachine mechanism
            // in order to pass the evaluator instance to the builder
            // only used when evaluating async methods from release builds.
            _evaluator = evaluator;
        }

        public void SetResult(TResult result)
        {
            Debug.Assert(_evaluator != null);
            _evaluator!.SetResult(result);
        }

        public void SetException(Exception exception)
        {
            Debug.Assert(_evaluator != null);
            _evaluator!.SetException(exception);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_evaluator != null);
            _evaluator!.SetAwaiter(ref awaiter);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            Debug.Assert(_evaluator != null);
            _evaluator!.SetAwaiter(ref awaiter);
        }
    }
}
