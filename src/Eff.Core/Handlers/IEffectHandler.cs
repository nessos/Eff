using System.Threading.Tasks;

namespace Nessos.Eff
{
    public interface IEffectHandler 
    {
        Task Handle<TResult>(EffEffAwaiter<TResult> awaiter);
        Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter);
        Task Handle<TResult>(TaskEffAwaiter<TResult> awaiter);

        Task<TResult> Handle<TResult>(ResultEff<TResult> setResultEff);
        Task Handle<TResult>(ExceptionEff<TResult> setExceptionEff);
        Task<Eff<TResult>> Handle<TResult>(DelayEff<TResult> delayEff);
        Task<Eff<TResult>> Handle<TResult>(AwaitEff<TResult> awaitEff);
    }
}
