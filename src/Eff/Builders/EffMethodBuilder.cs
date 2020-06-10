using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Builders
{
    public class EffMethodBuilder<TResult> : EffMethodBuilderBase<TResult>
    {
        public Eff<TResult>? Task => _eff;

        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public void SetStateMachine(IAsyncStateMachine _)
        {

        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _state = stateMachine;
            _cloner = StateMachineCloner<EffMethodBuilder<TResult>,TStateMachine>.Cloner;
            _eff = new DelayEff<TResult>(this);
        }

        public void SetResult(TResult result)
        {
            _eff = new ResultEff<TResult>(result, _state!);
        }

        public void SetException(Exception exception)
        {
            _eff = new ExceptionEff<TResult>(exception, _state!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : EffAwaiterBase
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : EffAwaiterBase
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }
    }

    public class EffMethodBuilder : EffMethodBuilderBase<Unit>
    {
        public Eff? Task => _eff;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static EffMethodBuilder Create()
        {
            return new EffMethodBuilder();
        }

        public void SetStateMachine(IAsyncStateMachine _)
        {

        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _state = stateMachine;
            _cloner = StateMachineCloner<EffMethodBuilder, TStateMachine>.Cloner;
            _eff = new DelayEff<Unit>(this);
        }

        public void SetResult()
        {
            _eff = new ResultEff<Unit>(Unit.Value, _state!);
        }

        public void SetException(Exception exception)
        {
            _eff = new ExceptionEff<Unit>(exception, _state!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : EffAwaiterBase
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : EffAwaiterBase
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }
    }
}
