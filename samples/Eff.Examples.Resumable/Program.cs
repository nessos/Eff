using Nessos.Effects;
using Nessos.Effects.Handlers;
using System.Diagnostics;

// Define a resumable computation
async Eff ResumableWorkflow()
{
    for (int i = 0; i < 20; i++)
    {
        Console.WriteLine($"Iterate {i}");
        if (i % 7 == 6)
        {
            await SuspendEffect.Value; // Effect signaling suspension
        }
    }
}

// Execute using a resumable effect handler
var handler = new ResumableEffectHandler(ResumableWorkflow());

Console.WriteLine("Starting");
while (! await handler.TryResume())
{
    Console.WriteLine("Workflow suspended");
}
Console.WriteLine("Done");

//
//  Implementation details
//

public class SuspendEffect : Effect
{
    public static SuspendEffect Value { get; } = new();
}

public class ResumableEffectHandler : IEffectHandler
{
    private readonly EffStateMachine<Unit> _stateMachine;
    private EffectAwaiter? _suspendedAwaiter;

    public ResumableEffectHandler(Eff suspendableWorkflow)
    {
        _stateMachine = Eff.FromUntypedEff(suspendableWorkflow).GetStateMachine();
    }

    public bool IsSuspended => _suspendedAwaiter != null;

    // Executes the encapsulated workflow until a suspend effect is encountered.
    // Returns true if workflow has run to completion, false if suspended.
    public async ValueTask<bool> TryResume()
    {
        if (_suspendedAwaiter != null)
        {
            // State machine is suspended, set a result to the pending awaiter
            // before resuming state machine execution.
            _suspendedAwaiter.SetResult();
            _suspendedAwaiter = null;
        }

        await ((IEffectHandler)this).Handle(_stateMachine).ConfigureAwait(false);
        return _suspendedAwaiter == null;
    }

    ValueTask IEffectHandler.Handle<TResult>(EffectAwaiter<TResult> awaiter)
    {
        Debug.Assert(_suspendedAwaiter == null);
        if (awaiter is EffectAwaiter { Effect: SuspendEffect } suspendedAwaiter)
        {
            _suspendedAwaiter = suspendedAwaiter;
        }

        return default;
    }

    async ValueTask IEffectHandler.Handle<TResult>(EffStateMachine<TResult> stateMachine)
    {
        Debug.Assert(_suspendedAwaiter == null);
        while (_suspendedAwaiter == null)
        {
            stateMachine.MoveNext();

            switch (stateMachine.Position)
            {
                case StateMachinePosition.Result:
                case StateMachinePosition.Exception:
                    return;

                case StateMachinePosition.TaskAwaiter:
                    await stateMachine.TaskAwaiter!.Value.ConfigureAwait(false);
                    break;

                case StateMachinePosition.EffAwaiter:
                    await stateMachine.EffAwaiter!.Accept(this).ConfigureAwait(false);
                    break;

                default:
                    Debug.Fail($"Invalid state machine position {stateMachine.Position}.");
                    return;
            }
        }
    }
}