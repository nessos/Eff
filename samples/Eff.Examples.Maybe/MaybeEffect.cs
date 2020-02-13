namespace Nessos.Eff.Examples.Maybe
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

    public class MaybeEffect<T> : Effect<T>
    {
        public Maybe<T> Result { get; set; }
    }

    public static class MaybeEffect
    {
        public static MaybeEffect<T> Nothing<T>() => new MaybeEffect<T> { Result = Maybe<T>.Nothing };
        public static MaybeEffect<T> Just<T>(T t) => new MaybeEffect<T> { Result = Maybe<T>.Just(t) };
    }
}
