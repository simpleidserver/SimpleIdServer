using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.WalletClient.Services;
using System;

namespace SimpleIdServer.WalletClient;

public class VerifiableCredentialOfferResolverFactory
{
    public static IVerifiableCredentialResolver Build(Action<WalletClientOptions> options = null)
    {
        var services = new ServiceCollection();
        services.AddWalletClient(options);
        var serviceProvider = services.BuildServiceProvider();
        var resolver = serviceProvider.GetRequiredService<IVerifiableCredentialResolver>();
        return resolver;
    }
}
