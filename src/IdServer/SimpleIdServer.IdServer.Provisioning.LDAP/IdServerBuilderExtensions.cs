// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Provisioning.LDAP.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace SimpleIdServer.IdServer.Provisioning.LDAP;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Adds support for LDAP provisioning services.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IdServerBuilder AddLdapProvisioning(this IdServerBuilder builder)
    {
        builder.Services.AddTransient<IIdProviderAuthService, LDAPAuthenticationService>();
        builder.Services.AddTransient<IProvisioningService, LDAPProvisioningService>();
        builder.AutomaticConfigurationOptions.Add<LDAPRepresentationsExtractionJobOptions>();
        return builder;
    }
}
