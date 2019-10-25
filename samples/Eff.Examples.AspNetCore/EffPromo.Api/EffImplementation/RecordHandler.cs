using System.Collections.Generic;
using System.Threading.Tasks;

using Eff.Effects;

namespace Nessos.EffPromo.Api.EffImplementation
{
	using System;

	using Eff.Core;

	using Microsoft.Extensions.Logging;

	using Newtonsoft.Json;

	public class EffectResult
	{
		public object Value;
		public Type Type;
	}

	public class RecordHandler : CtxEffectHandler
	{
		public RecordHandler(IEffCtx ctx, ILogger<RecordHandler> logger)
			: base(ctx)
		{
			Logger = logger;
		}

		private List<EffectResult> results = new List<EffectResult>();

		private ILogger<RecordHandler> Logger { get; }

		public override async Task Handle<TResult>(IEffect<TResult> effect)
		{
			await base.Handle(effect);
			results.Add(new EffectResult {Value = effect.Result, Type = typeof(TResult)});
		}

		public void DumpEffect() => Logger.LogInformation("Effect handing: {@Result}",
			JsonConvert.SerializeObject(
				results,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				}));
	}
}
