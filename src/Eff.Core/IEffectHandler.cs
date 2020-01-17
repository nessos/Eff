using System.Threading.Tasks;

namespace Nessos.Eff
{
    public interface IEffectHandler 
    {
        Task Handle<TResult>(EffEffAwaiter<TResult> awaiter);
        Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter);
        Task Handle<TResult>(TaskEffAwaiter<TResult> awaiter);

        Task<TResult> Handle<TResult>(SetResult<TResult> setResultEff);
        Task Handle<TResult>(SetException<TResult> setExceptionEff);
        Task<Eff<TResult>> Handle<TResult>(Delay<TResult> delayEff);
        Task<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff);
    }
}
