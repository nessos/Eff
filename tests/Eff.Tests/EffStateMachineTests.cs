namespace Nessos.Effects.Tests;

using Nessos.Effects.Handlers;
using Xunit;

public static class EffStateMachineTests
{
    [Fact]
    public static void NewStateMachine_Position_ShouldReturnNotStarted()
    {
        var stateMachine = Test().GetStateMachine();

        Assert.Equal(StateMachinePosition.NotStarted, stateMachine.Position);
        Assert.Null(stateMachine.Exception);
        Assert.Null(stateMachine.EffAwaiter);
        Assert.Null(stateMachine.TaskAwaiter);
        Assert.False(stateMachine.IsCompleted);

        async Eff<int> Test()
        {
            await Task.Delay(10);
            return 42;
        }
    }

    [Fact]
    public static void ReturnedValue_Position_ShouldReturnResult()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

        Assert.Equal(StateMachinePosition.Result, stateMachine.Position);
        Assert.Equal(42, stateMachine.Result);
        Assert.Null(stateMachine.Exception);
        Assert.Null(stateMachine.EffAwaiter);
        Assert.Null(stateMachine.TaskAwaiter);
        Assert.True(stateMachine.IsCompleted);

        async Eff<int> Test() => 42;
    }

    [Fact]
    public static void Exception_Position_ShouldReturnException()
    {
        var exn = new DivideByZeroException();
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

        Assert.Equal(StateMachinePosition.Exception, stateMachine.Position);
        Assert.Equal(exn, stateMachine.Exception);
        Assert.Null(stateMachine.EffAwaiter);
        Assert.Null(stateMachine.TaskAwaiter);
        Assert.True(stateMachine.IsCompleted);

        async Eff<int> Test() => throw exn;
    }

    [Fact]
    public static void OnAwaitedEff_Position_ShouldReturnEffAwaiter()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

        Assert.Equal(StateMachinePosition.EffAwaiter, stateMachine.Position);
        Assert.Null(stateMachine.Exception);
        Assert.IsAssignableFrom<EffStateMachine<int>>(stateMachine.EffAwaiter);
        Assert.Null(stateMachine.TaskAwaiter);
        Assert.False(stateMachine.IsCompleted);

        async Eff<int> Test()
        {
            return await Eff.FromResult(42);
        }
    }

    [Fact]
    public static void OnAwaitedEffect_Position_ShouldReturnEffAwaiter()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

        Assert.Equal(StateMachinePosition.EffAwaiter, stateMachine.Position);
        Assert.Null(stateMachine.Exception);
        Assert.IsAssignableFrom<EffectAwaiter<int>>(stateMachine.EffAwaiter);
        Assert.Null(stateMachine.TaskAwaiter);
        Assert.False(stateMachine.IsCompleted);

        async Eff<int> Test()
        {
            return await new TestEffect<int>();
        }
    }

    [Fact]
    public static void OnTaskAwaiter_Position_ShouldReturnTask()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

        Assert.Equal(StateMachinePosition.TaskAwaiter, stateMachine.Position);
        Assert.Null(stateMachine.Exception);
        Assert.Null(stateMachine.EffAwaiter);
        Assert.NotNull(stateMachine.TaskAwaiter);
        Assert.False(stateMachine.IsCompleted);

        async Eff<int> Test()
        {
            return await new TaskCompletionSource<int>().Task;
        }
    }

    [Fact]
    public static async Task OnTaskAwaiter_Completed_ShouldBeAbleToAdvanceStateMachine()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Assert.NotNull(stateMachine.TaskAwaiter);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        await stateMachine.TaskAwaiter!.Value;
        stateMachine.MoveNext();
        Assert.Equal(42, stateMachine.Result);

        async Eff<int> Test()
        {
            await Task.Delay(1_000);
            return 42;
        }
    }

    [Fact]
    public static async Task OnTaskAwaiter_Delay_ShouldReturnCorrectResult()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Assert.NotNull(stateMachine.TaskAwaiter);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        await stateMachine.TaskAwaiter!.Value;
        stateMachine.MoveNext();

        Assert.Equal(42, stateMachine.Result);

        async Eff<int> Test()
        {
            return await TaskMethod();

            async Task<int> TaskMethod()
            {
                await Task.Delay(1000);
                return 42;
            }
        }
    }

    [Fact]
    public static async Task OnTaskAwaiter_CanSubscribeMultipleCallbacks()
    {
        var stateMachine = Test().GetStateMachine();
        stateMachine.MoveNext();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Assert.NotNull(stateMachine.TaskAwaiter);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        var awaitTask = stateMachine.TaskAwaiter!.Value;

        int counter = 0;

        async Task Await()
        {
            await awaitTask;
            Interlocked.Increment(ref counter);
        }

        // check while the task is still running
        await Task.WhenAll(Enumerable.Range(1, 20).Select(_ => Await()));
        Assert.Equal(20, counter);

        // check after the task has completed
        await Task.WhenAll(Enumerable.Range(1, 20).Select(_ => Await()));
        Assert.Equal(40, counter);

        async Eff<int> Test()
        {
            await Task.Delay(2_000);
            return 42;
        }
    }

    private class TestEffect<T> : Effect<T>
    {

    }
}