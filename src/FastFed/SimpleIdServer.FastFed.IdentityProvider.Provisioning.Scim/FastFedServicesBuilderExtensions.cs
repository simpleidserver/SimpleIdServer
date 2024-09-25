// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddIdProviderScimProvisioning(this FastFedServicesBuilder builder)
    {
        builder.Services.AddTransient<IIdProviderProvisioningService, ScimIdProviderProvisioningService>();
        return builder;
    }
}
