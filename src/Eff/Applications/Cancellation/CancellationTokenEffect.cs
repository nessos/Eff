namespace Nessos.Effects.Cancellation;

/// <summary>
///   An abstract effect returning a cancellation token.
/// </summary>
public class CancellationTokenEffect : Effect<CancellationToken>
{
    /// <summary>
    ///   Gets the singleton cancellation token effect.
    /// </summary>
    public static Effect<CancellationToken> Value { get; } = new CancellationTokenEffect();
}
