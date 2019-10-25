using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nessos.EffPromo.Api.EffImplementation
{
	using Eff.Effects;

	using Microsoft.Extensions.DependencyInjection;

	public class EffectContext : IEffCtx
	{
		public EffectContext(IServiceProvider provider)
		{
			Provider = provider;
		}

		private IServiceProvider Provider { get; }

		public T Resolve<T>() => Provider.GetRequiredService<T>();
	}
}
