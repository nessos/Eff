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
        private EffEvaluator<Unit>? _evaluator;

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
            // in order to pass the evaluator instance to the builder
            // only used when evaluating async methods from release builds.
            _evaluator = (EffEvaluator<Unit>)stateMachine;
        }

        void IEffMethodBuilder<Unit>.SetEvaluator(EffEvaluator<Unit> evaluator)
        {
            _evaluator = evaluator;
        }

        public void SetResult()
        {
            Debug.Assert(_evaluator != null);
            _evaluator!.SetResult(Unit.Value);
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
