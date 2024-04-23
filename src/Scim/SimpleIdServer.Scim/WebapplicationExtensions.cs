// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;

namespace Microsoft.AspNetCore.Builder;

public static class WebapplicationExtensions
{
    public static WebApplication UseScim(this WebApplication webApplication)
    {
        var opts = webApplication.Services.GetRequiredService<IOptions<SCIMHostOptions>>().Value;
        var usePrefix = opts.EnableRealm;

        webApplication.MapControllerRoute("getResourceTypes",
            pattern: SCIMEndpoints.ResourceType,
            defaults: new { controller = "ResourceTypes", action = "GetAll" });
        webApplication.MapControllerRoute("getResourceType",
            pattern: SCIMEndpoints.ResourceType + "/{id}",
            defaults: new { controller = "ResourceTypes", action = "Get" });

        webApplication.MapControllerRoute("getSchemas",
            pattern: SCIMEndpoints.ResourceType,
            defaults: new { controller = "Schemas", action = "GetAll" });
        webApplication.MapControllerRoute("getSchemas",
            pattern: SCIMEndpoints.ResourceType + "/{id}",
            defaults: new { controller = "Schema", action = "GetAll" });

        webApplication.MapControllerRoute("getServiceProviderConfig",
            pattern: SCIMEndpoints.ServiceProviderConfig,
            defaults: new { controller = "ServiceProviderConfig", action = "Get" });

        // add users
        // add groups

        return webApplication;
    }
}
