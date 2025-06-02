using SimpleIdServer.Authzen.Rego;
using SimpleIdServer.Authzen.Rego.Compiler;
using SimpleIdServer.Authzen.Rego.Discover;
using SimpleIdServer.Authzen.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRegoEvaluator(this IServiceCollection services, Action<RegoEvaluatorOptions>? cb = null)
    {
        if (cb != null)
        {
            services.Configure(cb);
        }
        else
        {
            services.Configure<RegoEvaluatorOptions>((o) => { });
        }

        services.AddTransient<IAuthzenPolicyEvaluator, RegoEvaluator>();
        services.AddTransient<IRegoPathResolver, RegoPathResolver>();
        services.AddTransient<IRegoPoliciesResolver, RegoPoliciesResolver>();
        return services;
    }

    internal static IServiceCollection AddRegoClientDownloader(this IServiceCollection services, Action<RegoDownloaderOptions>? cb = null)
    {
        if (cb != null)
        {
            services.Configure(cb);
        }
        else
        {
            services.Configure<RegoDownloaderOptions>((o) => { });
        }

        services.AddTransient<IOpaDownloader, OpaDownloader>();
        services.AddTransient<IOpaPathResolver, OpaPathResolver>();
        services.AddHttpClient();
        return services;
    }
}
