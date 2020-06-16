using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nessos.Effects.Builders
{
    public abstract class EffStateMachine<TResult>
    {
        protected IAsyncStateMachine? _asyncStateMachine;
        protected Func<EffStateMachine<TResult>, EffStateMachine<TResult>>? _cloner;
        protected Eff<TResult>? _currentEff;

        /// <summary>
        ///   Exposes the underlying async state machine object for metadata use.
        /// </summary>
        public object State
        {
            get
            {
                Debug.Assert(_asyncStateMachine != null, "The state machine has not been initialized.");
                return _asyncStateMachine!;
            }
        }

        /// <summary>
        ///   Advances the state machine to its next stage.
        /// </summary>
        /// <returns>The Eff value denoting the next state of the computation.</returns>
        public Eff<TResult> MoveNext()
        {
            Debug.Assert(_asyncStateMachine != null, "The state machine has not been initialized.");
            _asyncStateMachine!.MoveNext();
            return _currentEff!;
        }

        /// <summary>
        ///   Creates a cloned instance of the eff state machine.
        /// </summary>
        public EffStateMachine<TResult> Clone()
        {
            Debug.Assert(_cloner != null, "The state machine has not been initialized.");
            return _cloner!(this);
        }

        /// <summary>
        ///   Reflection-driven state machine cloner
        /// </summary>
        protected static class StateMachineCloner<TBuilder, TStateMachine> where TBuilder : EffStateMachine<TResult>, new()
                                                                           where TStateMachine : IAsyncStateMachine
        {
            private static volatile bool s_isInitialized = false;
            private static MethodInfo? s_memberwiseCloner; 
            private static FieldInfo? s_smBuilder;

            public static readonly Func<EffStateMachine<TResult>, TBuilder> Cloner = Clone;

            private static TBuilder Clone(EffStateMachine<TResult> builder)
            {
                if (!s_isInitialized)
                {
                    Initialize();
                }

                var clonedBuilder = new TBuilder();
                // clone the state machine and point the `<>t__builder` field to the new builder instance.
                var clonedStateMachine = (IAsyncStateMachine)s_memberwiseCloner!.Invoke(builder._asyncStateMachine, null);
                s_smBuilder?.SetValue(clonedStateMachine, clonedBuilder);
                clonedBuilder._asyncStateMachine = clonedStateMachine;
                clonedBuilder._cloner = builder._cloner;
                return clonedBuilder;
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
}
