using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Nessos.Effects.Handlers;

namespace Nessos.Effects.Builders
{
    public abstract class EffMethodBuilderBase<TResult> : IEffStateMachine<TResult>
    {
        protected IAsyncStateMachine? _state;
        protected Func<EffMethodBuilderBase<TResult>, EffMethodBuilderBase<TResult>>? _cloner;
        protected Eff<TResult>? _eff;

        Eff<TResult> IEffStateMachine<TResult>.MoveNext(bool useClonedStateMachine)
        {
            var builder = useClonedStateMachine ? _cloner!(this) : this;
            builder._state!.MoveNext();
            return builder._eff!;
        }

        object IEffStateMachine<TResult>.State => _state!;

        protected void AwaitOnCompletedCore<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : EffAwaiterBase
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.SetState(_state!);
            _eff = new AwaitEff<TResult>(awaiter, this);
        }

        /// <summary>
        ///   Reflection-driven state machine cloner
        /// </summary>
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
                    throw new InvalidOperationException("Cannot clone uninitialized state machine builder.");
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
