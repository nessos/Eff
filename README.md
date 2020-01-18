# Eff [![Build Status](https://travis-ci.org/nessos/Eff.svg?branch=master)](https://travis-ci.org/nessos/Eff) [![NuGet Badge](https://buildstats.info/nuget/Eff)](https://www.nuget.org/packages/Eff/)
A library for programming with effects and handlers in C#, inspired by the [Eff] programming language 
and the implementation of Algebraic Effects in [OCaml], [Eff Directly in OCaml]. 
Effects are a powerful language feature that can be used to implement dependency injection, 
exception handling, nondeterministic computation, trace logging and much more.

## Introduction

The Eff library takes advantage of the [async method extensibility features](https://devblogs.microsoft.com/premier-developer/dissecting-the-async-methods-in-c/) available since C# 7.
At its core, the library defines a task-like type, `Eff<TResult>`, which can be built using `async` methods:

```csharp
async Eff HelloWorld()
{
    Console.WriteLine($"Hello, {await Helper()}!");

    async Eff<string> Helper() => "World";
}
```

Note that unlike `Task`, `Eff` types have cold semantics and so running

```csharp
Eff foo = Foo();
```

will have no observable side-effect in stdout.
An `Eff` instance has to be run explicitly by passing an _effect handler_:

```csharp
foo.Run(new DefaultEffectHandler()); // "Hello, World!"
```

So what is the benefit of using a convoluted version of regular async methods?

### Programming with Effects

A key concept of the Eff library are _abstract effects_:

```csharp
public class CoinToss : Effect<bool>
{

}
```

`Eff` methods are capable of consuming abstract effects:

```csharp
async Eff TossNCoins(int n)
{
    for (int i = 0; i < n; i++)
    {
        bool result = await new CoinToss();
        Console.WriteLine($"Got {(result ? "Heads" : "Tails")}");
    }
}
```

So how do we run this method now?
The answer is we need write an effect handler that _interprets the abstract effect_:
    
```csharp
public class RandomCoinTossHandler : EffectHandler
{
    private readonly Random _random = new Random();

    public override async Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter)
    {
        switch (awaiter)
        {
            case EffectEffAwaiter<bool> { Effect: CoinToss _ } awtr:
                awtr.SetResult(_random.NextDouble() < 0.5);
                break;
        }
    }
}
```

We can then execute the method by passing the handler:

```csharp
TossNCoins(100).Run(new RandomCoinTossHandler()); // prints random sequence of Heads and Tails
```

Note that we can reuse the same method using other interpretations of the effect:

```csharp
public class BiasedCoinTossHandler : EffectHandler
{
    private readonly Random _random = new Random();

    public override async Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter)
    {
        switch (awaiter)
        {
            case EffectEffAwaiter<bool> { Effect: CoinToss _ } awtr:
                awtr.SetResult(_random.NextDouble() < 0.01);
                break;
        }
    }
}

TossNCoins(100).Run(new BiasedCoinTossHandler()); // prints sequence of mostly Tails
```

Please see the [samples folder](https://github.com/nessos/Eff/tree/master/samples) for more examples of Eff applications.

[Eff]: http://math.andrej.com/wp-content/uploads/2012/03/eff.pdf
[OCaml]: http://www.lpw25.net/ocaml2015-abs2.pdf
[Eff Directly in OCaml]: http://kcsrk.info/papers/eff_ocaml_ml16.pdf
