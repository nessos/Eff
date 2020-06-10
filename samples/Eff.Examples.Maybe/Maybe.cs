namespace Nessos.Effects.Examples.Maybe
{
    public struct Maybe<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        private Maybe(T value)
        {
            HasValue = true;
            Value = value;
        }

        public static Maybe<T> Nothing { get; } = new Maybe<T>();
        public static Maybe<T> Just(T value) => new Maybe<T>(value);
    }
}
