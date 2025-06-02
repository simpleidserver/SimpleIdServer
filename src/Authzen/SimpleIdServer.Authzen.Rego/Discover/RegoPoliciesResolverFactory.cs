using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Authzen.Rego.Discover;

public class RegoPoliciesResolverFactory
{
    public static IRegoPoliciesResolver? Build(Action<RegoEvaluatorOptions>? cb = null)
    {
        var services = new ServiceCollection();
        services.AddRegoEvaluator(cb);
        return services.BuildServiceProvider().GetRequiredService<IRegoPoliciesResolver>();
    }
}
