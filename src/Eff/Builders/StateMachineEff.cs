using Nessos.Effects.Handlers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   An Eff instance holding a suspended async state machine.
    /// </summary>
    internal sealed class StateMachineEff<TBuilder, TStateMachine, TResult> : Eff<TResult>
        where TStateMachine : IAsyncStateMachine
        where TBuilder : IEffMethodBuilder<TResult>, new()
    {
        private readonly TStateMachine _stateMachine;

        public StateMachineEff(in TStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override EffStateMachine<TResult> GetAwaiter()
        {
            return new EffStateMachine<TBuilder, TStateMachine, TResult>(in _stateMachine);
        }
    }

    internal sealed class EffStateMachine<TBuilder, TStateMachine, TResult> : EffStateMachine<TResult>, IAsyncStateMachine
        where TStateMachine : IAsyncStateMachine
        where TBuilder : IEffMethodBuilder<TResult>, new()
    {
        private TStateMachine _stateMachine;

        public EffStateMachine(in TStateMachine stateMachine)
        {
            // Debug builds of async methods will generally produce class state machines,
            // while Release builds generate struct machines.
            // In the latter case state machine cloning can be achieved by simple struct copying
            // and taking advantage of the SetStateMachine() method.
            // However for objects we have to fall back to reflection. 
            // This probably fine since it should only concern Debug builds.

            if (null != (object?)default(TStateMachine)) // JIT optimization magic
            {
                // state machine is a struct, will be copied
                _stateMachine = stateMachine;
                _stateMachine.SetStateMachine(this); // pass the eff state machine instance to the underlying method builder
            }
            else
            {
                // state machine is an object, use reflection to create a shallow copy
                _stateMachine = ReflectionHelpers.Clone(this, stateMachine);
            }
        }

        public override void MoveNext() => _stateMachine.MoveNext();

        public override EffStateMachine<TResult> Clone()
        {
            return new EffStateMachine<TBuilder, TStateMachine, TResult>(in _stateMachine);
        }

        void IAsyncStateMachine.MoveNext()
        {
            throw new NotSupportedException();
        }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _)
        {
            throw new NotSupportedException();
        }

        public override IAsyncStateMachine? GetStateMachine() => _stateMachine;

        private static class ReflectionHelpers
        {
            private static volatile bool s_isInitialized = false;
            private static MethodInfo? s_memberwiseCloner;
            private static FieldInfo? s_smBuilder;

            /// <summary>
            ///   Provides a reflection-driven workaround for cloning class state machines.
            ///   Should only be needed in Debug builds.
            /// </summary>
            public static TStateMachine Clone(EffStateMachine<TResult> effStateMachine, TStateMachine stateMachine)
            {
                if (!s_isInitialized)
                {
                    Debug.Assert(typeof(TBuilder).IsValueType && !typeof(TStateMachine).IsValueType);
                    Initialize();
                }

                // Create a memberwise clone of the heap allocated state machine
                var clonedStateMachine = (TStateMachine)s_memberwiseCloner!.Invoke(stateMachine, null);
                // Create a new method builder copy and initialize it with our eff state machine
                var newBuilder = new TBuilder();
                newBuilder.SetStateMachine(effStateMachine);
                // Store a copy of the new method builder in the cloned state machine
                s_smBuilder!.SetValue(clonedStateMachine, newBuilder);
                return clonedStateMachine;
            }

            private static void Initialize()
            {
                s_memberwiseCloner = typeof(TStateMachine).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                s_smBuilder = typeof(TStateMachine)
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(f => f.FieldType == typeof(TBuilder))
                    .FirstOrDefault();

                s_isInitialized = true;
            }
        }
    }

    // Abstracts EffMethodBuilder types
    internal interface IEffMethodBuilder<TResult>
    {
        void SetStateMachine(EffStateMachine<TResult> stateMachine);
    }
}
