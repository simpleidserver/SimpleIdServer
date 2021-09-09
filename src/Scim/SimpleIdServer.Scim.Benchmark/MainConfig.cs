using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;

namespace SimpleIdServer.Scim.Benchmark
{
    public class MainConfig : ManualConfig
    {
        public MainConfig()
        {
            Add(RPlotExporter.Default);
        }
    }
}
