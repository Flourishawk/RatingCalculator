using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MyBenchmark;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;

namespace MyApp
{
    internal class MainBenchmarkDotNet
    {
        static void Main()
        {
            var config = ManualConfig.CreateEmpty()
                .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance))
                .AddLogger(ConsoleLogger.Default)
                .AddExporter(CsvExporter.Default)
                .AddDiagnoser(MemoryDiagnoser.Default)
                .AddColumn(StatisticColumn.Mean,
                           StatisticColumn.Error,
                           StatisticColumn.StdDev,
                           StatisticColumn.OperationsPerSecond,
                           StatisticColumn.Iterations,
                           TargetMethodColumn.Method,
                           RankColumn.Arabic,
                           BaselineAllocationRatioColumn.RatioMean
                           );

            BenchmarkRunner.Run<MyFirstBench>(config);
        }
    }
}