namespace Nessos.EffPromo.Api.Controllers
{
	using System.Threading.Tasks;

	using Eff.Core;

	using EffImplementation;

	using Microsoft.AspNetCore.Mvc;

	using Nessos.EffPromo.Api.Base;

	using Microsoft.Extensions.Logging;

	using Nessos.EffPromo.Persistence;
	using System.Collections.Generic;

	[Route("api/replay")]
	public class ReplayEffectsController : BaseController<ReplayEffectsController>
	{
		public ReplayEffectsController(
			ILogger<ReplayEffectsController> logger,
			ILogger<RecordHandler> handlerLogger,
			EffDbContext dbContext,
			EffectContext effCtx,
			AuthorsController ctr,
			IEnumerable<ControllerBase> controllers)
			: base(logger, handlerLogger, dbContext, effCtx)
		{
			Authors = ctr;
			Controllers = controllers;
		}

		private IEnumerable<ControllerBase> Controllers { get; }

		private AuthorsController Authors { get; }

		[HttpGet("")]
		public async Task<ActionResult> SimpleTest()
		{
			return Ok();
		}

		[HttpPost("")]
		public async Task<ActionResult> ReplayGetAuthors(string effectJson)
		{
			var replayHandler = new ReplayHandler(effectJson);
			var result = await Authors.GetAuthorsEff().Run(replayHandler);
			return Ok(result);
		}
	}
}
