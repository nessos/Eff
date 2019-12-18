namespace Nessos.EffPromo.Api.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Nessos.Eff;

	using EffImplementation;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using Nessos.EffPromo.Api.Base;
	using Nessos.EffPromo.Persistence;
	using Nessos.EffPromo.Persistence.Model;

	[Route("api/authors")]
	public class AuthorsController : BaseController<AuthorsController>
	{


		[HttpGet("")]
		public async Task<ActionResult<List<Author>>> GetAuthors()
		{
			var result = await GetAuthorsEff().Run(RecordHandler);
			RecordHandler.DumpEffect();
			return Ok(result);
		}

		[HttpGet("eff")]
		public async Eff<List<Author>> GetAuthorsEff()
		{
			// using var scope = Logger.BeginScope($"{nameof(AuthorsController)}.{nameof(GetAuthors)}");

			var authors = await IO.Do(ctx => ctx.Resolve<EffDbContext>()
				.Authors
				.TagWith($"Retrieving authors for {nameof(GetAuthors)}")
				.ToListAsync());

			//Logger.LogCritical("Found {Count} authors: {@Authors}", authors.Count, authors);
			return authors;
		}

		public AuthorsController(ILogger<AuthorsController> logger, ILogger<RecordHandler> handlerLogger, EffDbContext dbContext, EffectContext effCtx) : base(logger, handlerLogger, dbContext, effCtx)
		{
		}
	}
}
