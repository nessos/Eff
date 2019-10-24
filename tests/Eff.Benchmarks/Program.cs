using System;
using BenchmarkDotNet.Running;

namespace Eff.Benchmarks
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
