using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    /// <summary>
    ///   A state machine holding an eff method computation
    /// </summary>
    public abstract class EffStateMachine
    {
        internal EffStateMachine() { }

        /// <summary>
        ///   Gets a heap allocated copy of the underlying compiler-generated state machine, for metadata use.
        /// </summary>
        public abstract object GetState();
    }

    /// <summary>
    ///   A state machine holding an eff method computation
    /// </summary>
    public abstract class EffStateMachine<TResult> : EffStateMachine
    {
        protected Eff<TResult>? _currentEff;

        internal EffStateMachine() { }

        /// <summary>
        ///   Advances the state machine to its next stage.
        /// </summary>
        /// <returns>The Eff value denoting the next state of the computation.</returns>
        public abstract Eff<TResult> MoveNext();

        /// <summary>
        ///   Creates a cloned copy of the eff state machine.
        /// </summary>
        public abstract EffStateMachine<TResult> Clone();

        internal void SetEff(Eff<TResult> eff)
        {
            _currentEff = eff;
        }
    }

    internal sealed class EffStateMachine<TBuilder, TStateMachine, TResult> : EffStateMachine<TResult>, IAsyncStateMachine
        where TStateMachine : IAsyncStateMachine
        where TBuilder : IEffMethodBuilder<TResult>, new()
    {
        private readonly TStateMachine _stateMachine;

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
                _stateMachine.SetStateMachine(this); // pass the state machine to the underlying method builder
            }
            else
            {
                // state machine is an object, use reflection to create a shallow copy
                _stateMachine = ReflectionHelpers.Clone(this, stateMachine);
            }
        }

        public override Eff<TResult> MoveNext()
        {
            _stateMachine.MoveNext();
            Debug.Assert(_currentEff != null);
            return _currentEff!;
        }

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

        public override object GetState() => _stateMachine;

        private static class ReflectionHelpers
        {
            private static volatile bool s_isInitialized = false;
            private static MethodInfo? s_memberwiseCloner;
            private static FieldInfo? s_smBuilder;

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
                // Store a copy of the new method builder to the state machine
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

    // Holds a heap-allocated copy of the state machine passed to the method builder.
    // Can be thought as a factory for EffStateMachines.
    internal sealed class DelayEff<TBuilder, TStateMachine, TResult> : DelayEff<TResult>
        where TStateMachine : IAsyncStateMachine
        where TBuilder : IEffMethodBuilder<TResult>, new()
    {
        private TStateMachine _stateMachine;

        public DelayEff(in TStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override EffStateMachine<TResult> CreateStateMachine()
        {
            return new EffStateMachine<TBuilder, TStateMachine, TResult>(in _stateMachine);
        }
    }

    internal interface IEffMethodBuilder<TResult>
    {
        void SetStateMachine(EffStateMachine<TResult> stateMachine);
    }
}
