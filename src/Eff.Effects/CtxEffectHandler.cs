using System;
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

		public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
		{
			switch (awaiter.Effect)
			{
				case DoEffect<TResult> doEffect:
					Exception? error = null;
					TResult result = default!;

					try
                    {
						result = await doEffect.Func(ctx);
                    }
					catch (Exception e)
                    {
						error = e;
                    }

					if (error is null)
                    {
						awaiter.SetResult(result);
                    }
					else
                    {
						awaiter.SetException(error);
				    }

					break;
			}
		}
	}
}
