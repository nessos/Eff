using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nessos.Eff;

namespace Nessos.EffPromo.Api.EffImplementation
{
	using Newtonsoft.Json;

	public class ReplayHandler : EffectHandler
	{
		public ReplayHandler(string effectJson)
		{
			Results = JsonConvert.DeserializeObject<List<EffectResult>>(effectJson,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				});
		}

		private List<EffectResult> Results { get; }

		private int i = 0;

		public override async Task Handle<TResult>(EffectAwaiter<TResult> effect)
		{
			var result = Results[i++];
			effect.SetResult((TResult)Convert.ChangeType(result.Value, result.Type));
		}
	}
}
