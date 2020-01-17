using System.Threading.Tasks;

namespace Nessos.Eff
{
    public class DefaultEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(EffectEffAwaiter<TResult> effect) => Task.CompletedTask;
    }
}
