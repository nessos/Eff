namespace Nessos.Effects.Examples.Continuation;

public static class Effects
{
    /// <summary>
    ///  Call-With-Current-Continuation as an abstract effect.
    /// </summary>
    /// <remarks>
    ///   Cf. https://en.wikipedia.org/wiki/Call-with-current-continuation
    /// </remarks>
    public static Effect<T> CallCC<T>(Func<Func<T, Task>, Func<Exception, Task>, Task> body) => new CallCcEffect<T>(body);
}

/// <summary>
///  Call-With-Current-Continuation as an abstract effect.
/// </summary>
/// <remarks>
///   Cf. https://en.wikipedia.org/wiki/Call-with-current-continuation
/// </remarks>
public class CallCcEffect<T> : Effect<T>
{
    public CallCcEffect(Func<Func<T, Task>, Func<Exception, Task>, Task> body)
    {
        Body = body;
    }

    public Func<Func<T, Task>, Func<Exception, Task>, Task> Body { get; }
}
