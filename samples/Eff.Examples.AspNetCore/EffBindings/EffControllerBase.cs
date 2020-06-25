namespace Nessos.Effects.Examples.AspNetCore.EffBindings
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    [TypeFilter(typeof(EffExceptionFilter))]
    public abstract class EffControllerBase : ControllerBase
    {
        protected IEffectHandlerFactory EffectHandlerFactory { get; }

        protected EffControllerBase(IEffectHandlerFactory factory)
        {
            EffectHandlerFactory = factory;
        }

        /// <summary>
        ///   Executes an Eff computation using a new effect handler supplied by the controller factory.
        /// </summary>
        protected async Task<T> Execute<T>(Eff<T> eff)
        {
            await using var handler = EffectHandlerFactory.Create(ControllerContext);
            return await eff.Run(handler);
        }

        /// <summary>
        ///   Executes an Eff computation using a new effect handler supplied by the controller factory.
        /// </summary>
        protected async Task Execute(Eff eff)
        {
            await using var handler = EffectHandlerFactory.Create(ControllerContext);
            await eff.Run(handler);
        }
    }

    /// <summary>
    ///   Intercepts controller method exceptions while preserving the original HttpContext
    /// </summary>
    public class EffExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<EffExceptionFilter> _logger;

        public EffExceptionFilter(ILogger<EffExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // intercept exceptions here to preserve any replay tokens in response headers
            context.HttpContext.Response.StatusCode = 500;
            context.ExceptionHandled = true;
            _logger.LogError(context.Exception, "Unhandled exception");
        }
    }
}
