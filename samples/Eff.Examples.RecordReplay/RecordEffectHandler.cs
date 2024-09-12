namespace Nessos.Effects.Examples.RecordReplay;

using Nessos.Effects.DependencyInjection;
using Nessos.Effects.Handlers;

public class RecordEffectHandler : DependencyEffectHandler
{
    private readonly List<RecordedResult> _results = new List<RecordedResult>();

    public RecordEffectHandler(IContainer dependencies) : base(dependencies)
    {

    }

    public override async ValueTask Handle<TResult>(EffectAwaiter<TResult> awaiter)
    {
        await base.Handle(awaiter);
        var result = RecordedResult.FromAwaiter(awaiter);
        _results.Add(result);
    }

    public RecordedResult[] GetReplayLog() => _results.ToArray();
}
