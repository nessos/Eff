using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

using Eff.Core;

namespace Eff.Effects
{
	public class CtxEffectHandler : EffectHandler
	{
		protected readonly IEffCtx ctx;

		public CtxEffectHandler(IEffCtx ctx)
		{
			this.ctx = ctx;
		}

		public override async Task Handle<TResult>(IEffect<TResult> effect)
		{
			switch (effect)
			{
				case DoEffect<TResult> _effect:
					_effect.SetResult(await _effect.Func(ctx));
					break;
			}
		}
	}
}
