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

        public Eff<TResult>? Task { get; private set; }

        private EffMethodBuilder() 
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static EffMethodBuilder<TResult> Create()
        {
            return new EffMethodBuilder<TResult>();
        }

        public Eff<TResult> Trigger()
        {
            // ensure original state machine is never run
            // this is to guarantee thread safety of delayed Eff instances
            var builder = _isClonedInstance ? this : CloneBuilder();
            builder._state!.MoveNext();
            return builder.Task!;
        }

        public object State
        {
            get => _state!;
            set => _state = (IAsyncStateMachine)value;
        }

        public IContinuation<TResult> Clone() => CloneBuilder();

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _state = stateMachine;
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

        private EffMethodBuilder<TResult> CloneBuilder()
        {
            if (_state is null)
            {
                throw new InvalidOperationException("Cannot clone uninitialized state machine builder");
            }

            var clonedBuilder = new EffMethodBuilder<TResult>();
            var stateMachineRefl = s_cloneRefl.GetOrAdd(_state.GetType(), GetMembers);
            var clonedStateMachine = (IAsyncStateMachine)stateMachineRefl.memberwiseClone.Invoke(_state, null);
            stateMachineRefl.builder?.SetValue(clonedStateMachine, clonedBuilder);
            clonedBuilder._state = clonedStateMachine;
            clonedBuilder._isClonedInstance = true;
            return clonedBuilder;

            static (MethodInfo, FieldInfo?) GetMembers(Type type)
            {
                var memberwiseClone = type.GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                var field = type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => f.FieldType == typeof(EffMethodBuilder<TResult>)).FirstOrDefault();
                return (memberwiseClone, field);
            }
        }

        private static readonly ConcurrentDictionary<Type, (MethodInfo memberwiseClone, FieldInfo? builder)> s_cloneRefl = new ConcurrentDictionary<Type, (MethodInfo, FieldInfo?)>();
    }
}
