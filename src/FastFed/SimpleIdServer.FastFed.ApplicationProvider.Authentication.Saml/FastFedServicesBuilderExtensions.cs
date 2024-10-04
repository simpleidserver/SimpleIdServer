// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;
using SimpleIdServer.IdServer.Saml.Sp;
using System;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddSamlAppProviderAuthenticationProfile(this FastFedServicesBuilder builder, Action<SamlAuthenticationOptions> cb)
    {
        builder.Services.Configure(cb);
        var opts = new SamlAuthenticationOptions();
        cb(opts);
        builder.Services.Configure<SamlSpOptions>(o =>
        {
            o.BackchannelHttpHandler = opts.BackchannelHttpHandler;
            o.SigningCertificate = opts.SigningCertificate;
            o.ContactPersons = opts.ContactPersons;
            o.SPId = opts.SpId;
        });
        builder.Services.AddTransient<IProviderMetadataEnricher, SamlAuthenticationProviderMetadataEnricher>();
        builder.Services.AddTransient<IAppProviderProvisioningService, SamlAuthenticationProvisioningService>();
        builder.Services.AddScoped<IAuthenticationHandlerProvider, DynamicSamlAuthenticationHandlerProvider>();
        builder.Services.AddSingleton<IAuthenticationSchemeProvider, DynamicSamlAuthenticationSchemeProvider>();
        builder.Services.AddSingleton<ISamlAuthenticationSchemeProvider>(x => x.GetService<IAuthenticationSchemeProvider>() as ISamlAuthenticationSchemeProvider);
        return builder;
    }
}