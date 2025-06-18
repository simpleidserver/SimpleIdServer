using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Authzen.Rego.Compiler;

public static class OpaCompilerFactory
{
    public static IOpaCompiler? Build(Action<RegoOptions>? act = null)
    {
        var services = new ServiceCollection();
        if (act != null)
        {
            services.Configure(act);
        }
        else
        {
            services.Configure<RegoOptions>((o) =>
            {
                
            });
        }

        services.AddRegoCompiler();
        services.AddRegoDiscovery();
        return services.BuildServiceProvider().GetRequiredService<IOpaCompiler>();
    }
}
