using System;
using Microsoft.Extensions.DependencyInjection;
using Nessos.Eff;

namespace Nessos.EffPromo.Api.EffImplementation
{
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
