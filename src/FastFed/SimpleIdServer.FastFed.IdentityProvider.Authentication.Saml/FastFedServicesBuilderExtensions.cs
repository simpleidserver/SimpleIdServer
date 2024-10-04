// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.FastFed.IdentityProvider.Services;
using System;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddIdProviderSamlAuthentication(this FastFedServicesBuilder builder, Action<FastFedSamlAuthenticationOptions> cb)
    {
        builder.Services.Configure(cb);
        builder.Services.AddTransient<IFastFedEnricher, SamlAuthenticationFastFedEnricher>();
        builder.Services.AddTransient<IIdProviderProvisioningService, SamlIdProviderProvisioningService>();
        return builder;
    }
}
