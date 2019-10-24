``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18363
Intel Core i7-8665U CPU 1.90GHz (Coffee Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100-preview1-014459
  [Host] : .NET Core 3.1.0-preview1.19506.1 (CoreCLR 4.700.19.50403, CoreFX 4.700.19.50410), 64bit RyuJIT


```
|           Method | Mean | Error | Ratio | RatioSD |
|----------------- |-----:|------:|------:|--------:|
| &#39;Native Methods&#39; |   NA |    NA |     ? |       ? |
|   &#39;Task Builder&#39; |   NA |    NA |     ? |       ? |
|    &#39;Eff Builder&#39; |   NA |    NA |     ? |       ? |

Benchmarks with issues:
  Benchmark.'Native Methods': DefaultJob
  Benchmark.'Task Builder': DefaultJob
  Benchmark.'Eff Builder': DefaultJob
