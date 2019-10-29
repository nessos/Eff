#pragma warning disable 1998

using System.Threading.Tasks;

namespace Eff.Core
{
    public class DefaultEffectHandler : EffectHandler
    {
        public override async Task Handle<TResult>(IEffect<TResult> effect)
        {
            
        }
    }
}
