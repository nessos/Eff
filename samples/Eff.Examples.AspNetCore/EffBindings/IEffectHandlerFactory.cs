namespace Nessos.Effects.Examples.AspNetCore.EffBindings
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Nessos.Effects.Handlers;

    /// <summary>
    ///   An effect handler that must be disposed at the end of an MVC controller lifetime.
    /// </summary>
    public interface IDisposableEffectHandler : IEffectHandler, IAsyncDisposable
    {

    }

    /// <summary>
    ///   Abstract factory for constructing MVC effect handlers.
    /// </summary>
    public interface IEffectHandlerFactory
    {
        IDisposableEffectHandler Create(ControllerContext ctx); 
    }
}
