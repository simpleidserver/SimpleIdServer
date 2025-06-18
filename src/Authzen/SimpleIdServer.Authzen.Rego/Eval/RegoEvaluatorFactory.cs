using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Authzen.Services;

namespace SimpleIdServer.Authzen.Rego.Eval;

public class RegoEvaluatorFactory
{
    public static IAuthzenPolicyEvaluator? Build(Action<RegoOptions>? act = null)
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

        services.AddLogging(b =>
        {
            b.AddConsole();
        });
        services.AddRegoEvaluator();
        services.AddRegoDiscovery();
        return services.BuildServiceProvider().GetRequiredService<IAuthzenPolicyEvaluator>();
    }
}
