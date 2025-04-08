// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseScim(this WebApplication webApp, List<string> additionalRoutes = null)
    {
        webApp.UseAuthentication();
        webApp.UseAuthorization();
        var usePrefix = webApp.Services.GetRequiredService<IOptions<ScimHostOptions>>().Value.EnableRealm;
        webApp.UseMvc(o =>
        {
            if(additionalRoutes != null)
            {
                foreach (var additionalRoute in additionalRoutes)
                {
                    o.UseStandardScimEdp(additionalRoute, usePrefix);
                }
            }

            o.ScimMapControllerRoute("getResourceTypes",
                pattern: SCIMEndpoints.ResourceType,
                defaults: new { controller = "ResourceTypes", action = "GetAll" });
            o.ScimMapControllerRoute("getResourceType",
                pattern: SCIMEndpoints.ResourceType + "/{id}",
                defaults: new { controller = "ResourceTypes", action = "Get" });

            o.ScimMapControllerRoute("getSchemas",
                pattern: SCIMEndpoints.Schemas,
                defaults: new { controller = "Schemas", action = "GetAll" });
            o.ScimMapControllerRoute("getSchema",
                pattern: SCIMEndpoints.Schemas + "/{id}",
                defaults: new { controller = "Schemas", action = "Get" });

            o.ScimMapControllerRoute("getServiceProviderConfig",
                pattern: SCIMEndpoints.ServiceProviderConfig,
                defaults: new { controller = "ServiceProviderConfig", action = "Get" });
            o.ScimMapControllerRoute("bulk",
                pattern: $"{(usePrefix ? "{prefix}/" : string.Empty)}{SCIMEndpoints.Bulk}",
                defaults: new { controller = "Bulk", action = "Index" });

            o.UseStandardScimEdp(SCIMEndpoints.User, usePrefix);
            o.UseStandardScimEdp(SCIMEndpoints.Group, usePrefix);

            o.MapRoute("catchAllGet",
                template: "{*url}",
                defaults: new { controller = "Default", action = "Get" });
            o.MapRoute("catchAllPut",
                template: "{*url}",
                defaults: new { controller = "Default", action = "Put" });
            o.MapRoute("catchAllPost",
                template: "{*url}",
                defaults: new { controller = "Default", action = "Post" });
            o.MapRoute("catchAllDelete",
                template: "{*url}",
                defaults: new { controller = "Default", action = "Delete" });
            o.MapRoute("catchAllPatch",
                template: "{*url}",
                defaults: new { controller = "Default", action = "Patch" });
        });
        return webApp;
    }
}