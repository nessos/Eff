namespace Nessos.Effects.Examples.RecordReplay;

using Nessos.Effects.Handlers;

public class ReplayEffectHandler : EffectHandler
{
    private readonly RecordedResult[] _results;
    private int _index = 0;

    public ReplayEffectHandler(IEnumerable<RecordedResult> results)
    {
        _results = results.ToArray();
    }

    public override ValueTask Handle<TResult>(EffectAwaiter<TResult> effect)
    {
        if (_index == _results.Length)
        {
            throw new InvalidOperationException();
        }

        var result = _results[_index++];
        result.ToAwaiter(effect);
        return default;
    }
}
