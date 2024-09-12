namespace Nessos.Effects.DependencyInjection;

/// <summary>
///   Generic dependency resolver abstraction.
/// </summary>
public interface IContainer
{
    /// <summary>
    ///   Attempt to resolve a dependency of provided type.
    /// </summary>
    TDependency Resolve<TDependency>();
}
