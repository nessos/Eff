# Eff
A library design for programming with effects and handlers in C# 7, inspired by the [Eff] programming language and the implementation of Algebraic Effects in [OCaml], [Eff Directly in OCaml]. Eff can handle effect interpretation, detailed exception tracing and trace logging.

``` csharp
// Effect example
async Eff<int> Foo()
{
    var y = await Effect.Random();
    return y;
}
    
// Effect handler
public class CustomEffectHandler : EffectHandler
{
    private readonly Random random;
    public CustomEffectHandler(Random random)
    {
        this.random = random;
    }

    public override async Task Handle<TResult>(IEffect<TResult> effect)
    {
        switch (effect)
        {
            case RandomEffect randomEffect:
                randomEffect.SetResult(random.Next());
                break;
        }
    }
}

// Set effect handler and execute
var handler = new CustomEffectHandler(new Random());
var x = Foo().Run(handler).Result;
```

## Install
via [NuGet](https://www.nuget.org/packages/Eff):
```
PM> Install-Package Eff
```

[Eff]: http://math.andrej.com/wp-content/uploads/2012/03/eff.pdf
[OCaml]: http://www.lpw25.net/ocaml2015-abs2.pdf
[Eff Directly in OCaml]: http://kcsrk.info/papers/eff_ocaml_ml16.pdf
