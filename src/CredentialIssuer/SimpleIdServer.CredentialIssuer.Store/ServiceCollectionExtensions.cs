// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Store;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStore(this IServiceCollection services, Action<DbContextOptionsBuilder>? action = null)
    {
        var lifetime = ServiceLifetime.Scoped;
        RegisterDependencies(services);
        if (action != null) services.AddDbContext<CredentialIssuerDbContext>(action, lifetime);
        else services.AddDbContext<CredentialIssuerDbContext>(o => o.UseInMemoryDatabase("credentialIssuer"), lifetime);
        return services;
    }

    private static void RegisterDependencies(IServiceCollection services)
    {
        services.AddTransient<ICredentialOfferStore, CredentialOfferStore>();
        services.AddTransient<ICredentialConfigurationStore, CredentialConfigurationStore>();
        services.AddTransient<ICredentialStore, CredentialStore>();
        services.AddTransient<IUserCredentialClaimStore, UserCredentialClaimStore>();
    }
}