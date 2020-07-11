#pragma warning disable 1998
using BenchmarkDotNet.Attributes;
using Nessos.Effects.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Nessos.Effects.Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        const int ResultOffset = 50_000; // prevent task caching from kicking in
        private readonly int[] _data = Enumerable.Range(0, 100).Select(x => x + ResultOffset).ToArray();
        private readonly IEffectHandler _handler = new DefaultEffectHandler();

        [Benchmark(Description = "Task Builder", Baseline = true)]
        public async ValueTask TaskBuilder() => await TaskFlow.SumOfOddSquares(_data);

        [Benchmark(Description = "Eff Builder")]
        public async ValueTask EffBuilder() => await EffFlow.SumOfOddSquares(_data).Run(_handler);

        private static class TaskFlow
        {
            public static async Task<int> SumOfOddSquares(int[] inputs)
            {
                int sum = 0;
                foreach(var i in inputs)
                    if(i % 2 == 1) sum += await Square(i);

                return sum;

                static async Task<int> Square(int x) => await Echo(x) * await Echo(x);
                static async Task<T> Echo<T>(T x) => x;
            }
        }

        private static class EffFlow
        {
            public static async Eff<int> SumOfOddSquares(int[] inputs)
            {
                int sum = 0;
                foreach (var i in inputs)
                    if (i % 2 == 1) sum += await Square(i);

                return sum;

                static async Eff<int> Square(int x) => await Echo(x) * await Echo(x);
                static async Eff<T> Echo<T>(T x) => x;
            }
        }
    }
}
