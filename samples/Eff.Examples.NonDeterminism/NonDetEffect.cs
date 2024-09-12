namespace Nessos.Effects.Examples.NonDeterminism;

public class NonDetEffect<T> : Effect<T>
{
    public NonDetEffect(T[] choices)
    {
        Choices = choices;
    }

    public T[] Choices { get; }
}

public static class NonDetEffect
{
    /// Defines a nondeterminism effect which runs the eff continuation for every provided result
    public static NonDetEffect<T> Choose<T>(params T[] choices)
    {
        return new NonDetEffect<T>(choices);
    }
}
