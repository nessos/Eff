﻿using System;
using System.Runtime.CompilerServices;
using System.Security;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   Untyped Eff method builder
    /// </summary>
    public class EffMethodBuilder : EffStateMachine<Unit>
    {
        public Eff? Task => _currentEff;

        public static EffMethodBuilder Create()
        {
            return new EffMethodBuilder();
        }

        public void SetStateMachine(IAsyncStateMachine _)
        {
            throw new NotSupportedException();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _asyncStateMachine = stateMachine;
            _cloner = StateMachineCloner<EffMethodBuilder, TStateMachine>.Cloner;
            _currentEff = new DelayEff<Unit>(this);
        }

        public void SetResult()
        {
            _currentEff = new ResultEff<Unit>(Unit.Value, _asyncStateMachine!);
        }

        public void SetException(Exception exception)
        {
            _currentEff = new ExceptionEff<Unit>(exception, _asyncStateMachine!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.SetState(_asyncStateMachine!);
            _currentEff = new AwaitEff<Unit>(awaiter, this);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.SetState(_asyncStateMachine!);
            _currentEff = new AwaitEff<Unit>(awaiter, this);
        }
    }
}
