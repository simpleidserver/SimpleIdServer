// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.FastFed.Apis.FastFedMetadata;
using System;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddAppProviderScimProvisioning(this FastFedServicesBuilder builder, Action<ScimProvisioningOptions> cb)
    {
        builder.Services.Configure(cb);
        builder.Services.RemoveAll<IGetProviderMetadataQuery>();
        builder.Services.AddTransient<IAppProviderProvisioningService, ScimProvisioningService>();
        builder.Services.AddTransient<IGetProviderMetadataQuery, ScimGetProviderMetadataQuery>();
        return builder;
    }
}
