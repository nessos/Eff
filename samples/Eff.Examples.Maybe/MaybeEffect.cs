namespace Nessos.Effects.Examples.Maybe
{

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
