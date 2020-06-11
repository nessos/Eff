using System;
using System.Threading.Tasks;

namespace Nessos.Effects.DependencyInjection
{
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

    /// <summary>
    ///   Represents an effect that performs an operation against a supplied dependency container
    /// </summary>
    public abstract class DependencyEffect<TResult> : Effect<TResult>
    {
        public abstract ValueTask<TResult> Handle(IContainer container);
    }

    internal class FuncDependencyEffect<TDependency, TResult> : DependencyEffect<TResult>
    {
        public Func<TDependency, ValueTask<TResult>> Func { get; }

        public FuncDependencyEffect(Func<TDependency, ValueTask<TResult>> func)
        {
            Func = func;
        }

        public override ValueTask<TResult> Handle(IContainer container)
        {
            var dependency = container.Resolve<TDependency>();
            return Func(dependency);
        }
    }

    internal class ContainerFuncDependencyEffect<TResult> : DependencyEffect<TResult>
    {
        public Func<IContainer, ValueTask<TResult>> Func { get; }

        public ContainerFuncDependencyEffect(Func<IContainer, ValueTask<TResult>> func)
        {
            Func = func;
        }

        public override ValueTask<TResult> Handle(IContainer container) => Func(container);
    }
}
