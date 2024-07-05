// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Rp.Federation;
using SimpleIdServer.Rp.Federation.Builders;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRpFederation(this IServiceCollection services, Action<RpFederationOptions> cbOptions)
    {
        services.Configure(cbOptions);
        services.AddTransient<IRpFederationEntityBuilder, RpFederationEntityBuilder>();
        return services;
    }
}