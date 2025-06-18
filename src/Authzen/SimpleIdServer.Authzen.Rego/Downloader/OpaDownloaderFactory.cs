
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Authzen.Rego;

public static class OpaDownloaderFactory
{
    public static IOpaDownloader? Build(Action<RegoOptions>? cb = null)
    {
        var services = new ServiceCollection();
        if (cb != null)
        {
            services.Configure(cb);
        }
        else
        {
            services.Configure<RegoOptions>((o) =>
            {

            });
        }
        
        services.AddRegoDiscovery();
        services.AddRegoClientDownloader();
        return services.BuildServiceProvider().GetRequiredService<IOpaDownloader>();
    }
}
