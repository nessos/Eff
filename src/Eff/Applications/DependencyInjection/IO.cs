using System;
using System.Threading.Tasks;

namespace Nessos.Effects.DependencyInjection
{
    /// <summary>
    ///   Defines a suite of abstract effects that perform actions against the specified dependency type.
    /// </summary>
    /// <typeparam name="TDependency">The dependency the abstract effect depends on.</typeparam>
    public static class IO<TDependency>
    {
        /// <summary>
        ///   Defines an abstract effect that performs an operation against the specified dependency type.
        /// </summary>
        /// <typeparam name="TResult">The result of the effectful computation.</typeparam>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<TResult> Do<TResult>(Func<TDependency, TResult> func)
        {
            return new FuncDependencyEffect<TDependency, TResult>(dep => new ValueTask<TResult>(func(dep)));
        }

        /// <summary>
        ///   Defines an abstract effect that performs an operation against the specified dependency type.
        /// </summary>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<Unit> Do(Action<TDependency> func)
        {
            return new FuncDependencyEffect<TDependency, Unit>(dep => { func(dep); return new ValueTask<Unit>(Unit.Value); });
        }

        /// <summary>
        ///   Defines an abstract effect that performs an operation against the specified dependency type.
        /// </summary>
        /// <typeparam name="TResult">The result of the effectful computation.</typeparam>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<TResult> Do<TResult>(Func<TDependency, Task<TResult>> func)
        {
            return new FuncDependencyEffect<TDependency, TResult>(async dep => await func(dep));
        }

        /// <summary>
        ///   Defines an abstract effect that performs an operation against the specified dependency type.
        /// </summary>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<Unit> Do(Func<TDependency, Task> func)
        {
            return new FuncDependencyEffect<TDependency, Unit>(async dep => { await func(dep); return Unit.Value; });
        }
    }

    /// <summary>
    ///   Defines a suite of abstract effects that perform actions against a dependency container.
    /// </summary>
    public static class IO
    {
        /// <summary>
        ///   Defines an abstract effect that performs an operation against a dependency container.
        /// </summary>
        /// <typeparam name="TResult">The result of the effectful computation.</typeparam>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<TResult> Do<TResult>(Func<IContainer, TResult> func)
        {
            return new ContainerFuncDependencyEffect<TResult>(c => new ValueTask<TResult>(func(c)));
        }

        /// <summary>
        ///   Defines an abstract effect that performs an operation against a dependency container.
        /// </summary>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<Unit> Do(Action<IContainer> func)
        {
            return new ContainerFuncDependencyEffect<Unit>(c => { func(c); return new ValueTask<Unit>(Unit.Value); });
        }

        /// <summary>
        ///   Defines an abstract effect that performs an operation against a dependency container.
        /// </summary>
        /// <typeparam name="TResult">The result of the effectful computation.</typeparam>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<TResult> Do<TResult>(Func<IContainer, Task<TResult>> func)
        {
            return new ContainerFuncDependencyEffect<TResult>(async c => await func(c));
        }

        /// <summary>
        ///   Defines an abstract effect that performs an operation against a dependency container.
        /// </summary>
        /// <param name="func">The effectful operation performed against the dependency.</param>
        public static DependencyEffect<Unit> Do(Func<IContainer, Task> func)
        {
            return new ContainerFuncDependencyEffect<Unit>(async c => { await func(c); return Unit.Value; });
        }
    }
}
