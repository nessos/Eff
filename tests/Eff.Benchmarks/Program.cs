using System;
using BenchmarkDotNet.Running;

namespace Nessos.Effects.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var switcher = new BenchmarkSwitcher(assembly);
            var summaries = switcher.Run(args);
        }
    }
}
