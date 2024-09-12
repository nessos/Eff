namespace Nessos.Effects.Examples.AspNetCore.EffBindings;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nessos.Effects.DependencyInjection;
using Nessos.Effects.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
///   An effect handler with the following features:
///   1. Provides effectful dependecy injection by wrapping an IServiceProvider instance.
///   2. Logs the results of effectful operations for future replay.
/// </summary>
public class RecordingEffectHandler : DependencyEffectHandler, IDisposableEffectHandler
{
    private readonly List<PersistedEffect> _results = new List<PersistedEffect>();
    private readonly EffectLogger _store;
    private readonly HttpResponse _response;

    public RecordingEffectHandler(IContainer ctx, HttpResponse response, EffectLogger store) : base(ctx)
    {
        _store = store;
        _response = response;
    }

    public override async ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
    {
        try
        {
            await base.Handle(awaiter);
        }
        catch (Exception e)
        {
            awaiter.SetException(e);
        }

        var persistedEffect = PersistedEffect.FromCompletedAwaiter(awaiter);
        _results.Add(persistedEffect);
    }

    /// Commit the log results and propagate the replay token.
    public void Commit()
    {
        string replayToken = _store.Commit(_results);
        Container.Resolve<ILogger<RecordingEffectHandler>>().LogInformation($"Eff Replay Token {replayToken}");
        _response.AddReplayTokenHeader(replayToken);
    }

    public async ValueTask DisposeAsync() => Commit();
}

public class RecordingEffectHandlerFactory : IEffectHandlerFactory
{
    private readonly ServiceProviderEffContext _effCtx;
    private readonly EffectLogger _store;

    public RecordingEffectHandlerFactory(IServiceProvider serviceProvider)
    {
        _effCtx = new ServiceProviderEffContext(serviceProvider);
        _store = serviceProvider.GetRequiredService<EffectLogger>();
    }

    public IDisposableEffectHandler Create(ControllerContext ctx)
    {
        return new RecordingEffectHandler(_effCtx, ctx.HttpContext.Response, _store);
    }

    private class ServiceProviderEffContext : IContainer
    {
        private readonly IServiceProvider _provider;

        public ServiceProviderEffContext(IServiceProvider provider)
        {
            _provider = provider;
        }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public T Resolve<T>() => _provider.GetRequiredService<T>();
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    }
}
