using Nessos.Effects.Handlers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nessos.Effects.DependencyInjection;

namespace Nessos.Effects.Examples.RecordReplay
{
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
}
