#pragma warning disable 1998
using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Eff.Core;

namespace Eff.Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        private int[] _data;
        private IEffectHandler _handler;

        [GlobalSetup]
        public void Setup()
        {
            _data = Enumerable.Range(1, 100).ToArray();
            _handler = new DefaultEffectHandler();
        }

        [Benchmark(Description = "Managed Methods", Baseline = true)]
        public void ManagedMethods() => ManagedFlow.SumOfOddSquares(_data);

        [Benchmark(Description = "Task Builder")]
        public Task TaskBuilder() => TaskFlow.SumOfOddSquares(_data);

        [Benchmark(Description = "Eff Builder")]
        public Task EffBuilder() => EffFlow.SumOfOddSquares(_data).Run(_handler);

        private static class ManagedFlow
        {
            public static int SumOfOddSquares(int[] inputs)
            {
                int sum = 0;
                foreach (var i in inputs) 
                    if (i % 2 == 1) sum += Square(i);

                return sum;

                static int Square(int x) => Echo(x) * Echo(x);
                static T Echo<T>(T x) => x;
            }
        }

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
                    if (i % 2 == 1) sum += await Square(i).AsEffect();

                return sum;

                static async Eff<int> Square(int x) => await Echo(x).AsEffect() * await Echo(x).AsEffect();
                static async Eff<T> Echo<T>(T x) => x;
            }
        }
    }
}
