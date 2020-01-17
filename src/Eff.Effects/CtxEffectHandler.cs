using System.Threading.Tasks;

namespace Nessos.Eff
{
	public class CtxEffectHandler : EffectHandler
	{
		protected readonly IEffCtx ctx;

		public CtxEffectHandler(IEffCtx ctx)
		{
			this.ctx = ctx;
		}

		public override async Task Handle<TResult>(EffectEffAwaiter<TResult> awaiter)
		{
			switch (awaiter.Effect)
			{
				case DoEffect<TResult> doEffect:
					awaiter.SetResult(await doEffect.Func(ctx));
					break;
			}
		}
	}
}
