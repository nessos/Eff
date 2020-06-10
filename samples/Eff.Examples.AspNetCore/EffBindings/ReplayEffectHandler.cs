namespace Nessos.Effects.Examples.AspNetCore.EffBindings
{
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///   An effect handler that runs an effectful computation using a simplistic replay log.
    /// </summary>
    public class ReplayEffectHandler : EffectHandler, IMvcEffectHandler
    {
        private int _pos = 0;
        private readonly ImmutableArray<PersistedEffect> _replayResults;

        public ReplayEffectHandler(ImmutableArray<PersistedEffect> replayResults)
        {
            _replayResults = replayResults;
        }

        public override async Task Handle<TResult>(EffectAwaiter<TResult> awaiter)
        {
            if (_pos == _replayResults.Length)
            {
                throw new InvalidOperationException("Reached end of replay log.");
            }

            _replayResults[_pos++].WriteToAwaiter(awaiter);
        }

        public async ValueTask DisposeAsync()
        {

        }
    }

    /// <summary>
    ///   A record-or-replay effect handler factory that will either:
    ///   - Provide a replay effect handler if the incoming request has a valid Eff-Replay-Token header.
    ///   - Fall back to the regular dependency injection/recording effect handler otherwise.
    /// </summary>
    public class RecordReplayEffectHandlerFactory : IMvcEffectHandlerFactory
    {
        private readonly RecordingEffectHandlerFactory _recordingHandlerFactory;
        private readonly EffectLogger _effectLogStore;

        public RecordReplayEffectHandlerFactory(IServiceProvider provider)
        {
            _recordingHandlerFactory = new RecordingEffectHandlerFactory(provider);
            _effectLogStore = provider.GetRequiredService<EffectLogger>();
        }

        public IMvcEffectHandler Create(ControllerContext ctx)
        {
            if (ctx.HttpContext.Request.GetReplayTokenHeader() is string replayToken &&
                _effectLogStore.TryGetLogById(replayToken, out var replayLog))
            {
                return new ReplayEffectHandler(replayLog);
            }

            return _recordingHandlerFactory.Create(ctx);
        }
    }
}
