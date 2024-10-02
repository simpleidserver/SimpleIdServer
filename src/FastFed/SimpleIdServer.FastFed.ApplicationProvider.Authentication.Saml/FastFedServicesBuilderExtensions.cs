// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;
using System;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddSamlAppProviderAuthenticationProfile(this FastFedServicesBuilder builder, Action<SamlAuthenticationOptions> cb)
    {
        builder.Services.Configure(cb);
        builder.Services.AddTransient<IProviderMetadataEnricher, SamlAuthenticationProviderMetadataEnricher>();
        builder.Services.AddTransient<IAppProviderProvisioningService, SamlAuthenticationProvisioningService>();
        builder.Services.AddScoped<IAuthenticationHandlerProvider, DynamicSamlAuthenticationHandlerProvider>();
        builder.Services.AddSingleton<IAuthenticationSchemeProvider, DynamicSamlAuthenticationSchemeProvider>();
        builder.Services.AddSingleton<ISamlAuthenticationSchemeProvider>(x => x.GetService<IAuthenticationSchemeProvider>() as ISamlAuthenticationSchemeProvider);
        return builder;
    }
}