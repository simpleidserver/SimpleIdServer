using SimpleIdServer.Authzen.Rego;
using SimpleIdServer.Authzen.Rego.Compiler;
using SimpleIdServer.Authzen.Rego.Discover;
using SimpleIdServer.Authzen.Rego.Eval;
using SimpleIdServer.Authzen.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{    
    internal static IServiceCollection AddRegoEvaluator(this IServiceCollection services)
    {
        services.AddTransient<IAuthzenPolicyEvaluator, RegoEvaluator>();
        services.AddTransient<IWasmPolicyEvaluator, WasmPolicyEvaluator>();
        return services;
    }

    internal static IServiceCollection AddRegoCompiler(this IServiceCollection services)
    {
        services.AddTransient<IOpaCompiler, OpaCompiler>();
        return services;
    }

    internal static IServiceCollection AddRegoClientDownloader(this IServiceCollection services)
    {
        services.AddTransient<IOpaDownloader, OpaDownloader>();
        services.AddHttpClient();
        return services;
    }

    internal static IServiceCollection AddRegoDiscovery(this IServiceCollection services)
    {
        services.AddTransient<IOpaPathResolver, OpaPathResolver>();
        services.AddTransient<ICompiledOpaFilesResolver, CompiledOpaFilesResolver>();
        services.AddTransient<IRegoPathResolver, RegoPathResolver>();
        services.AddTransient<IRegoPoliciesResolver, RegoPoliciesResolver>();
        return services;
    }
}
