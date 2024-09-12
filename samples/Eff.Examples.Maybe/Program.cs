using Nessos.Effects;
using Nessos.Effects.Handlers;

await MaybeEffectHandler.Run(Test());

static async Eff Test()
{
    for (var i = 0; i < 100; i++)
    {
        await DivideAndReportToConsole(23, 5 - i);
    }
}

static async Eff DivideAndReportToConsole(int m, int n)
{
    Console.Write($"Calculating {m} / {n}: ");
    var result = await Divide(m, n);
    Console.WriteLine($"Got {result}!");
}

static async Eff<int> Divide(int m, int n)
{
    return (n == 0) ? await MaybeEffect.Nothing<int>() : await MaybeEffect.Just(m / n);
}

public readonly struct Maybe<T>
{
    public bool HasValue { get; }
    public T Value { get; }

    private Maybe(T value) => (HasValue, Value) = (true, value);

    public static Maybe<T> Nothing { get; } = new();
    public static Maybe<T> Just(T value) => new(value);
}

public class MaybeEffect<T> : Effect<T>
{
    public Maybe<T> Result { get; init; }
}

public static class MaybeEffect
{
    public static MaybeEffect<T> Nothing<T>() => new() { Result = Maybe<T>.Nothing };
    public static MaybeEffect<T> Just<T>(T t) => new() { Result = Maybe<T>.Just(t) };
}

public static class MaybeEffectHandler
{
    public static async Task<Maybe<TResult>> Run<TResult>(Eff<TResult> eff)
    {
        var stateMachine = eff.GetStateMachine();
        var handler = new MaybeEffectHandler<TResult>();
        await handler.Handle(stateMachine);

        return stateMachine.IsCompleted ?
            Maybe<TResult>.Just(stateMachine.GetResult()) :
            Maybe<TResult>.Nothing;
    }

    public static Task<Maybe<Unit>> Run(Eff eff)
    {
        return Run(Helper());

        async Eff<Unit> Helper() { await eff; return Unit.Value; }
    }
}

public class MaybeEffectHandler<TResult> : IEffectHandler
{
    private bool _breakExecution;

    public ValueTask Handle<TValue>(EffectAwaiter<TValue> awaiter)
    {
        switch (awaiter.Effect)
        {
            case MaybeEffect<TValue> { Result: { HasValue: true, Value: var value } }:
                awaiter.SetResult(value);
                break;

            case MaybeEffect<TValue>:
                _breakExecution = true;
                break;
        }

        return default;
    }

    public async ValueTask Handle<TValue>(EffStateMachine<TValue> stateMachine)
    {
        while (!_breakExecution)
        {
            stateMachine.MoveNext();

            switch (stateMachine)
            {
                case { Position: StateMachinePosition.Result or StateMachinePosition.Exception }:
                    return;

                case { Position: StateMachinePosition.TaskAwaiter, TaskAwaiter: { } awaiter }:
                    await awaiter;
                    break;

                case { Position: StateMachinePosition.EffAwaiter, EffAwaiter: { } awaiter }:
                    await awaiter.Accept(this);
                    break;

                default:
                    throw new Exception($"Invalid state machine position {stateMachine.Position}.");
            }
        }
    }
}
