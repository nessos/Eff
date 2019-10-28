using System.Threading.Tasks;

namespace Eff.Core
{
    public interface IEffectHandler 
    {
        Task Handle<TResult>(IEffect<TResult> effect);
        Task Handle<TResult>(TaskEffect<TResult> effect);
        Task Handle<TResult>(EffEffect<TResult> effect);

        Task<TResult> Handle<TResult>(SetResult<TResult> setResult);
        Task Handle<TResult>(SetException<TResult> setException);
        Task<Eff<TResult>> Handle<TResult>(Delay<TResult> delay);
        Task<Eff<TResult>> Handle<TResult>(Await<TResult> awaitEff);
    }
}
