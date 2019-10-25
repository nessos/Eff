namespace Nessos.EffPromo.Api.Base
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Nessos.EffPromo.Api.EffImplementation;
	using Nessos.EffPromo.Persistence;

	[ApiController]
	public class BaseController<T> : ControllerBase
		where T : BaseController<T>
	{
		// ReSharper disable once ContextualLoggerProblem
		public BaseController(ILogger<T> logger, ILogger<RecordHandler> handlerLogger, EffDbContext dbContext, EffectContext effCtx)
		{
			Logger = logger;
			DbContext = dbContext;
			RecordHandler = new RecordHandler(effCtx, handlerLogger);
		}

		protected ILogger<T> Logger { get; }

		protected RecordHandler RecordHandler { get; }

		protected EffDbContext DbContext { get; set; }
	}
}
