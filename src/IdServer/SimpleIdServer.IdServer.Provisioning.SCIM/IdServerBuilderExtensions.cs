// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Provisioning.SCIM.Services;

namespace SimpleIdServer.IdServer.Provisioning.SCIM;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Adds the SCIM provisioning service to the IdServer builder.
    /// </summary>
    /// <param name="builder">The IdServerBuilder to configure.</param>
    /// <returns>The updated IdServerBuilder instance.</returns>
    public static IdServerBuilder AddScimProvisioning(this IdServerBuilder builder)
    {
        builder.Services.AddTransient<IProvisioningService, SCIMProvisioningService>();
        builder.AutomaticConfigurationOptions.Add<SCIMRepresentationsExtractionJobOptions>();
        return builder;
    }
}
