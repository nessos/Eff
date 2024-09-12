namespace Nessos.Effects.DependencyInjection;

using Nessos.Effects.Handlers;

/// <summary>
///   Reference effect handler implementation that populates dependency effects with container contents.
/// </summary>
public class DependencyEffectHandler : EffectHandler
{
    /// <summary>
    ///   The container instance being used by the effect handler.
    /// </summary>
    protected IContainer Container { get; }

    /// <summary>
    ///   Creates a new Dependency injection effect handler using supplied container.
    /// </summary>
    /// <param name="container">The container containing required dependencies.</param>
    public DependencyEffectHandler(IContainer container)
    {
        Container = container;
    }

    /// <summary>
    ///   Handles awaiters containing a <see cref="DependencyEffect{TResult}"/>.
    /// </summary>
    public override async ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
    {
        switch (awaiter.Effect)
        {
            case DependencyEffect<TResult> depEffect:
                try
                {
                    TResult? result = await depEffect.Handle(Container).ConfigureAwait(false);
                    awaiter.SetResult(result);
                }
                catch (Exception e)
                {
                    awaiter.SetException(e);
                }

                break;
        }
    }
}
