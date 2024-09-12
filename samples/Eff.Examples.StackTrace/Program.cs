#pragma warning disable 1998
using Nessos.Effects;
using Nessos.Effects.Examples.StackTrace;

try
{
    var handler = new CustomEffectHandler();
    await Foo(0).Run(handler);
}
catch (Exception ex)
{
    Console.WriteLine(ex.StackTraceLog());
    Console.WriteLine(ex);
}

static async Eff<int> Baz(int x)
{
    return x + 1;
}

static async Eff<int> Bar(int x)
{
    return 1 / x;
}

static async Eff<int> Foo(int x)
{
    var y = await Baz(x).ConfigureAwait();
    var z = await Bar(x).ConfigureAwait();
    return y + z;
}