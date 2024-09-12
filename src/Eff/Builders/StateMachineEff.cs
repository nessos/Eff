namespace Nessos.Effects.Builders;

using Nessos.Effects.Handlers;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

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

    public override EffStateMachine<TResult> GetStateMachine()
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
            _stateMachine = ReflectionHelpers.Clone(stateMachine, this);
        }
    }

    public override void MoveNext() => _stateMachine.MoveNext();

    public override EffStateMachine<TResult> Clone()
    {
        EffStateMachine<TBuilder, TStateMachine, TResult> clone = new(in _stateMachine)
        {
            Position = Position,
            Exception = Exception,
            EffAwaiter = EffAwaiter,
            TaskAwaiter = TaskAwaiter
        };

        if (HasResult)
        {
            clone.SetResult(Result);
        }

        // recursively clone any parent state machines awaiting on the current instance
        if (AwaitingStateMachine is IEffStateMachine parent)
        {
            IEffStateMachine clonedParent = parent.Clone();
            clonedParent.UnsafeSetAwaiter(clone);
            clone.AwaitingStateMachine = clonedParent;
        }

        return clone;
    }

    public override void UnsafeSetAwaiter(EffAwaiter awaiter)
    {
        ReflectionHelpers.SetAwaiter(ref _stateMachine, awaiter);
        EffAwaiter = awaiter;
    }

    void IAsyncStateMachine.MoveNext()
    {
        throw new NotSupportedException();
    }

    void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine _)
    {
        throw new NotSupportedException();
    }

    public override IAsyncStateMachine GetAsyncStateMachine()
    {
        return default(TStateMachine) is not null ? _stateMachine : (IAsyncStateMachine)ReflectionHelpers.Clone(_stateMachine);
    }

    private static class ReflectionHelpers
    {
        private static readonly MethodInfo s_memberwiseCloner = typeof(TStateMachine)
                .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? s_smBuilder = typeof(TStateMachine)
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(TBuilder))
                .FirstOrDefault();

        private static readonly FieldInfo? s_smObject = typeof(TStateMachine)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => f.FieldType == typeof(object))
                .FirstOrDefault();

        /// <summary>
        ///   Provides a reflection-driven workaround for cloning class state machines.
        ///   Should only be needed in Debug builds.
        /// </summary>
        public static TStateMachine Clone(TStateMachine stateMachine, EffStateMachine<TResult> effStateMachine)
        {
            Debug.Assert(s_smBuilder != null);

            // Create a memberwise clone of the heap allocated state machine
            TStateMachine clonedStateMachine = (TStateMachine)s_memberwiseCloner.Invoke(stateMachine, null);
            // Create a new method builder copy and initialize it with our eff state machine
            TBuilder newBuilder = new();
            newBuilder.SetStateMachine(effStateMachine);
            // Store a copy of the new method builder in the cloned state machine
            s_smBuilder!.SetValue(clonedStateMachine, newBuilder);
            return clonedStateMachine;
        }

        public static TStateMachine Clone(TStateMachine stateMachine)
        {
            Debug.Assert(typeof(TBuilder).IsValueType && !typeof(TStateMachine).IsValueType);

            // Create a memberwise clone of the heap allocated state machine
            return (TStateMachine)s_memberwiseCloner!.Invoke(stateMachine, null);
        }

        public static void SetAwaiter(ref TStateMachine stateMachine, EffAwaiter awaiter)
        {
            Debug.Assert(s_smObject != null);

            if (null != (object?)default(TStateMachine)) // JIT optimization magic
            {
                IAsyncStateMachine boxedReplica = stateMachine;
                s_smObject!.SetValue(boxedReplica, awaiter);
                stateMachine = (TStateMachine)boxedReplica;
            }
            else
            {
                s_smObject!.SetValue(stateMachine, awaiter);
            }
        }
    }
}

// Abstracts EffMethodBuilder types
internal interface IEffMethodBuilder<TResult>
{
    void SetStateMachine(EffStateMachine<TResult> stateMachine);
}
