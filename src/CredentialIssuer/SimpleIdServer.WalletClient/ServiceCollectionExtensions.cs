using SimpleIdServer.Did.Key;
using SimpleIdServer.WalletClient;
using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.CredentialFormats;
using SimpleIdServer.WalletClient.Factories;
using SimpleIdServer.WalletClient.Services;
using SimpleIdServer.WalletClient.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWalletClient(this IServiceCollection services, Action<WalletClientOptions> callback = null)
    {
        if (callback != null) services.Configure(callback);
        else services.Configure<WalletClientOptions>(o => { });
        services.AddTransient<ISidServerClient, SidServerClient>();
        services.AddTransient<ICredentialIssuerClient, CredentialIssuerClient>();
        services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        services.AddTransient<IVerifiableCredentialsService, VerifiableCredentialService>();
        services.AddTransient<IVerifiableCredentialsService, ESBIVerifiableCredentialService> ();
        services.AddTransient<IDeferredCredentialIssuer, ESBIDeferredCredentialIssuer>();
        services.AddTransient<IDeferredCredentialIssuer, DeferredCredentialIssuer>();
        services.AddTransient<IVerifiableCredentialResolver, VerifiableCredentialResolver>();
        services.AddTransient<ICredentialFormatter, JwtVcFormatter>();
        services.AddTransient<ICredentialFormatter, LdpVcFormatter>();
        services.AddTransient<IVcStore, VcStore>();
        services.AddDidKey();
        return services;
    }
}
