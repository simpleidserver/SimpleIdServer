// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml.Sid;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddSidSamlAuthentication(this FastFedServicesBuilder builder, Action<FastFedSidSamlAuthenticationOptions> cb = null)
    {
        builder.Services.Configure(cb);
        builder.Services.AddTransient<ISamlClientProvisioningService, SidSamlClientProvisioningService>();
        return builder;
    }
}
