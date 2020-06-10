namespace Nessos.Effects.Examples.AspNetCore.EffBindings
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///   An effect handler that must be disposed at the end of an MVC controller lifetime.
    /// </summary>
    public interface IMvcEffectHandler : IEffectHandler, IAsyncDisposable
    {

    }

    /// <summary>
    ///   Abstract factory for constructing MVC effect handlers.
    /// </summary>
    public interface IMvcEffectHandlerFactory
    {
        IMvcEffectHandler Create(ControllerContext ctx); 
    }
}
