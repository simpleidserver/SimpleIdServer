using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.WalletClient.Services;
using SimpleIdServer.WalletClient.Stores;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.WalletClient;

public class VerifiableCredentialOfferResolverFactory
{
    public static IVerifiableCredentialResolver Build(Action<WalletClientOptions> options = null, List<StoredVcRecord> storedVcRecords = null)
    {
        var services = new ServiceCollection();
        services.AddWalletClient(options);
        services.AddSingleton<IVcStore>(new VcStore(storedVcRecords ?? new List<StoredVcRecord>()));
        var serviceProvider = services.BuildServiceProvider();
        var resolver = serviceProvider.GetRequiredService<IVerifiableCredentialResolver>();
        return resolver;
    }
}
