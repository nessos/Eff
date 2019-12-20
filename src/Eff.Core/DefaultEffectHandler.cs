using System.Threading.Tasks;

namespace Nessos.Eff
{
    public class DefaultEffectHandler : EffectHandler
    {
        public override Task Handle<TResult>(IEffect<TResult> effect) => Task.CompletedTask;
    }
}
