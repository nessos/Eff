using System;
using System.Threading.Tasks;

namespace Nessos.Effects.DependencyInjection
{
    public interface IEffCtx
    {
        T Resolve<T>();
    }
    
    public class DoEffect<T> : Effect<T>
    {
        public Func<IEffCtx, Task<T>> Func { get; }
        public DoEffect(Func<IEffCtx, Task<T>> func)
        {
            Func = func;
        }
    }

    public class DoEffect : DoEffect<Unit>
    {
        public DoEffect(Func<IEffCtx, Task> func) :
            base(async ctx => { await func(ctx); return Unit.Value; })
        {

        }
    }

    public static class IO
    {
        public static DoEffect<TResult> Do<TResult>(Func<IEffCtx, TResult> func)
        {
            return new DoEffect<TResult>(ctx => Task.FromResult(func(ctx)));
        }

        public static DoEffect Do(Action<IEffCtx> func)
        {
            return new DoEffect(ctx => { func(ctx); return Task.CompletedTask; });
        }

        public static DoEffect<TResult> Do<TResult>(Func<IEffCtx, Task<TResult>> func)
        {
            return new DoEffect<TResult>(func);
        }

        public static DoEffect Do(Func<IEffCtx, Task> func)
        {
            return new DoEffect(func);
        }
    }

    public static class IO<TDependency>
    {
        public static DoEffect<TResult> Do<TResult>(Func<TDependency, TResult> func)
        {
            return new DoEffect<TResult>(ctx => Task.FromResult(func(ctx.Resolve<TDependency>())));
        }

        public static DoEffect Do(Action<TDependency> func)
        {
            return new DoEffect(ctx => { func(ctx.Resolve<TDependency>()); return Task.CompletedTask; });
        }

        public static DoEffect<TResult> Do<TResult>(Func<TDependency, Task<TResult>> func)
        {
            return new DoEffect<TResult>(ctx => func(ctx.Resolve<TDependency>()));
        }

        public static DoEffect Do(Func<TDependency, Task> func)
        {
            return new DoEffect(ctx => func(ctx.Resolve<TDependency>()));
        }
    }
}
