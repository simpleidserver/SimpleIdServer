using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Authzen.Rego.Compiler;

public static class OpaCompilerFactory
{
    public static IOpaCompiler? Build(
        Action<RegoDownloaderOptions>? dl = null,
        Action<RegoEvaluatorOptions>? ev = null)
    {
        var services = new ServiceCollection();
        services.AddRegoClientDownloader(dl);
        services.AddRegoEvaluator(ev);
        services.AddTransient<IOpaCompiler, OpaCompiler>();
        return services.BuildServiceProvider().GetRequiredService<IOpaCompiler>();
    }
}
