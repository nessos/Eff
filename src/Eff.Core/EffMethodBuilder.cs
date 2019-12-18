using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.ComponentModel;

namespace Eff.Core
{
    public class EffMethodBuilder<TResult> : IContinuation<TResult>
    {
        private IAsyncStateMachine? _state;
        private bool _isClonedInstance = false;
        private Func<EffMethodBuilder<TResult>, EffMethodBuilder<TResult>>? _cloner;

        public Eff<TResult>? Task { get; private set; }

        private EffMethodBuilder() 
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        Eff<TResult> IContinuation<TResult>.Trigger()
        {
            // ensure original state machine is never run
            // to guarantee thread safety of delayed Eff instances
            var builder = _isClonedInstance ? this : _cloner!(this);
            builder._state!.MoveNext();
            return builder.Task!;
        }

        object IContinuation<TResult>.State
        {
            get => _state!;
            set => _state = (IAsyncStateMachine)value;
        }

        IContinuation<TResult> IContinuation<TResult>.Clone() => _cloner!(this);

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _state = stateMachine;
            _cloner = StateMachineCloner<TStateMachine>.Cloner;
            Task = new Delay<TResult>(this);
        }

        public void SetStateMachine(IAsyncStateMachine _)
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

        private void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _, bool safe)
            where TAwaiter : IEffect
            where TStateMachine : IAsyncStateMachine
        {

            switch (awaiter)
            {
                case IEffect effect:
                    effect.SetState(_state!);
                    Task = new Await<TResult>(effect, this);

                    break;
                default:
                    throw new EffException($"Awaiter {awaiter.GetType().Name} is not an effect. Try to use obj.AsEffect().");
            }
        }

        private static class StateMachineCloner<TStateMachine> where TStateMachine : IAsyncStateMachine
        {
            private static readonly MethodInfo s_memberwiseCloner = 
                typeof(TStateMachine).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            private static readonly FieldInfo? s_smBuilder =
                typeof(TStateMachine)
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(f => f.FieldType == typeof(EffMethodBuilder<TResult>))
                    .FirstOrDefault();

            public static readonly Func<EffMethodBuilder<TResult>, EffMethodBuilder<TResult>> Cloner = Clone;

            private static EffMethodBuilder<TResult> Clone(EffMethodBuilder<TResult> builder)
            {
                if (builder._state is null)
                {
                    throw new InvalidOperationException("Cannot clone uninitialized state machine builder");
                }

                var clonedBuilder = new EffMethodBuilder<TResult>();
                var clonedStateMachine = (IAsyncStateMachine)s_memberwiseCloner.Invoke(builder._state, null);
                s_smBuilder?.SetValue(clonedStateMachine, clonedBuilder);
                clonedBuilder._state = clonedStateMachine;
                clonedBuilder._isClonedInstance = true;
                return clonedBuilder;
            }
        }
    }
}
