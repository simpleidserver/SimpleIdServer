
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Authzen.Rego;

public static class OpaDownloaderFactory
{
    public static IOpaDownloader? Build(Action<RegoDownloaderOptions>? cb = null)
    {
        var services = new ServiceCollection();
        services.AddRegoClientDownloader(cb);
        return services.BuildServiceProvider().GetRequiredService<IOpaDownloader>();
    }
}
