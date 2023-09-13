// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Provisioning.LDAP.Jobs;
using SimpleIdServer.IdServer.Provisioning.LDAP.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Provisioning.LDAP;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddLDAPProvisioning(this IdServerBuilder builder)
    {
        builder.Services.AddTransient<IRepresentationExtractionJob, LDAPRepresentationsExtractionJob>();
        builder.Services.AddTransient<IIdProviderAuthService, LDAPAuthenticationService>();
        builder.Services.AddTransient<IProvisioningService, LDAPProvisioningService>();
        return builder;
    }
}
