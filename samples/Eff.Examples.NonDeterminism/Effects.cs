namespace Nessos.Effects.Examples.NonDeterminism
{
    public class NonDetEffect<T> : Effect<T>
    {
        public NonDetEffect(T[] choices)
        {
            Choices = choices;
        }

        public T[] Choices { get; }
    }

    public static class Effects
    {
        public static NonDetEffect<T> Choose<T>(params T[] choices)
        {
            return new NonDetEffect<T>(choices);
        }
    }
}
