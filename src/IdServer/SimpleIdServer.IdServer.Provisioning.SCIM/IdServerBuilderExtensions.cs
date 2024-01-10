// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Provisioning.SCIM.Services;

namespace SimpleIdServer.IdServer.Provisioning.SCIM;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddSCIMProvisioning(this IdServerBuilder builder)
    {
        builder.Services.AddTransient<IProvisioningService, SCIMProvisioningService>();
        return builder;
    }
}
