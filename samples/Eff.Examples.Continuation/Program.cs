using Nessos.Effects;
using Nessos.Effects.Examples.Continuation;

await NonDeterminism.Run(Test());

static async Eff Test()
{
    static async Eff<(int, string)> Nested()
    {
        var x = await NonDeterminism.Choice(1, 2, 3);
        var y = await NonDeterminism.Choice("a", "b", "c");
        return (x, y);
    }

    var (x, y) = await Nested();
    var z = await NonDeterminism.Choice(false, true);

    Console.WriteLine($"x = {x}, y = {y}, z = {z}");
}