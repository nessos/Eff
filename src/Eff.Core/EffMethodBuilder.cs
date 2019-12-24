using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.ComponentModel;

namespace Nessos.Eff
{
    public class EffMethodBuilder<TResult> : EffMethodBuilderBase<TResult>
    {
        public Eff<TResult>? Task => _eff;

        [EditorBrowsable(EditorBrowsableState.Never)]
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
            _eff = new Delay<TResult>(this);
        }

        public void SetResult(TResult result)
        {
            _eff = new SetResult<TResult>(result, _state!);
        }

        public void SetException(Exception exception)
        {
            _eff = new SetException<TResult>(exception, _state!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }


        //[SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : Awaiter
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
            _eff = new Delay<Unit>(this);
        }

        public void SetResult()
        {
            _eff = new SetResult<Unit>(Unit.Value, _state!);
        }

        public void SetException(Exception exception)
        {
            _eff = new SetException<Unit>(exception, _state!);
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }


        //[SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompletedCore(ref awaiter, ref stateMachine);
        }
    }

    public abstract class EffMethodBuilderBase<TResult> : IContinuation<TResult>
    {
        protected IAsyncStateMachine? _state;
        protected Func<EffMethodBuilderBase<TResult>, EffMethodBuilderBase<TResult>>? _cloner;
        protected Eff<TResult>? _eff;

        internal EffMethodBuilderBase()
        {

        }

        Eff<TResult> IContinuation<TResult>.Trigger(bool useClonedStateMachine)
        {
            var builder = useClonedStateMachine ? _cloner!(this) : this;
            builder._state!.MoveNext();
            return builder._eff!;
        }

        object IContinuation<TResult>.State
        {
            get => _state!;
            set => _state = (IAsyncStateMachine)value;
        }

        protected void AwaitOnCompletedCore<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : Awaiter
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.SetState(_state!);
            _eff = new Await<TResult>(awaiter, this);
        }

        protected static class StateMachineCloner<TBuilder, TStateMachine> where TStateMachine : IAsyncStateMachine
                                                                           where TBuilder : EffMethodBuilderBase<TResult>, new()
        {
            private static readonly MethodInfo s_memberwiseCloner =
                typeof(TStateMachine).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            private static readonly FieldInfo? s_smBuilder =
                typeof(TStateMachine)
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(f => f.FieldType == typeof(TBuilder))
                    .FirstOrDefault();

            public static readonly Func<EffMethodBuilderBase<TResult>, TBuilder> Cloner = Clone;

            private static TBuilder Clone(EffMethodBuilderBase<TResult> builder)
            {
                if (builder._state is null)
                {
                    throw new InvalidOperationException("Cannot clone uninitialized state machine builder");
                }

                var clonedBuilder = new TBuilder();
                // clone the state machine and point the `<>t__builder` field to the new builder instance.
                var clonedStateMachine = (IAsyncStateMachine)s_memberwiseCloner.Invoke(builder._state, null);
                s_smBuilder?.SetValue(clonedStateMachine, clonedBuilder);
                clonedBuilder._state = clonedStateMachine;
                clonedBuilder._cloner = builder._cloner;
                return clonedBuilder;
            }
        }
    }
}
