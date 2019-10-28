using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace Eff.Core
{
    public class EffMethodBuilder<TResult>
    {
        private object? _state;
        private Func<object, Eff<TResult>>? _continuation;

        public Eff<TResult>? Task { get; private set; }

        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _state = stateMachine;
            _continuation = state =>
            {
                _state = state;
                ((IAsyncStateMachine)state).MoveNext();
                return Task!;
            };
            Task = new Delay<TResult>(_continuation, _state);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            
        }

        public void SetResult(TResult result)
        {
            Task = new SetResult<TResult>(result, _state!);
        }

        public void SetException(Exception exception)
        {
            Task = new SetException<TResult>(exception, _state!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : IEffect
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine, true);
        }


        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : IEffect
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine, false);
        }

        private void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine, bool safe)
            where TAwaiter : IEffect
            where TStateMachine : IAsyncStateMachine
        {

            switch (awaiter)
            {
                case IEffect effect:
                    effect.SetState(_state!);
                    Task = new Await<TResult>(effect, _continuation!, _state!);

                    break;
                default:
                    throw new EffException($"Awaiter {awaiter.GetType().Name} is not an effect. Try to use obj.AsEffect().");
            }
        }
    }
}
