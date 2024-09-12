namespace Nessos.Effects.Builders;

using Nessos.Effects.Handlers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
///   Untyped Eff method builder.
/// </summary>
public struct EffMethodBuilder : IEffMethodBuilder<Unit>
{
    private EffStateMachine<Unit>? _effStateMachine;

    /// <summary>
    ///   Creates a new method builder. For use by the generated state machine.
    /// </summary>
    public static EffMethodBuilder Create() => default;

    /// <summary>
    ///   Returns the built eff instance. For use by the generated state machine.
    /// </summary>
    public Eff? Task { get; private set; }

    /// <summary>
    ///   Passes the async state machine to the method builder. For use by the generated state machine.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {
        Task = new StateMachineEff<EffMethodBuilder, TStateMachine, Unit>(in stateMachine);
    }

    /// <summary>
    ///   Sets a Eff state machine instance. For use by the generated state machine.
    /// </summary>
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // hijacks the IAsyncStateMachine.SetStateMachine mechanism
        // in order to pass the eff state machine instance to the builder
        // only used when evaluating async methods from release builds.
        _effStateMachine = (EffStateMachine<Unit>)stateMachine;
    }

    void IEffMethodBuilder<Unit>.SetStateMachine(EffStateMachine<Unit> stateMachine)
    {
        _effStateMachine = stateMachine;
    }

    /// <summary>
    ///   Sets the result of the computation. For use by the generated state machine.
    /// </summary>
    public readonly void SetResult()
    {
        Debug.Assert(_effStateMachine != null);
        _effStateMachine!.BuilderSetResult(Unit.Value);
    }

    /// <summary>
    ///   Sets an exception for the computation. For use by the generated state machine.
    /// </summary>
    public readonly void SetException(Exception exception)
    {
        Debug.Assert(_effStateMachine != null);
        _effStateMachine!.BuilderSetException(exception);
    }

    /// <summary>
    ///   Sets an awaiter instance. For use by the generated state machine.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        Debug.Assert(_effStateMachine != null);
        _effStateMachine!.BuilderSetAwaiter(ref awaiter);
    }

    /// <summary>
    ///   Sets an awaiter instance. For use by the generated state machine.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        Debug.Assert(_effStateMachine != null);
        _effStateMachine!.UnsafeBuilderSetAwaiter(ref awaiter);
    }
}
