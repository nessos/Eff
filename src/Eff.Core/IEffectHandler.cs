using System.Threading.Tasks;

namespace Nessos.Eff
{
    public interface IEffectHandler 
    {
        Task Handle<TResult>(EffectAwaiter<TResult> effect);
        Task Handle<TResult>(TaskAwaiter<TResult> effect);
        Task Handle<TResult>(EffAwaiter<TResult> effect);

        Task<TResult> Handle<TResult>(SetResult<TResult> setResult);
        Task Handle<TResult>(SetException<TResult> setException);
        Task<Eff<TResult>> Handle<TResult>(Delay<TResult> delay);
        Task<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff);
    }
}
