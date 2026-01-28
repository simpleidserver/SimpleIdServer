// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder;

public static class ScimApplicationBuilderExtensions
{
    public static IApplicationBuilder UseScim(this IApplicationBuilder app, List<string> additionalRoutes = null)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        var usePrefix = app.ApplicationServices.GetRequiredService<IOptions<ScimHostOptions>>().Value.EnableRealm;
        app.UseMvc(o =>
        {
            ConfigureScimRoutes(o, usePrefix, additionalRoutes);
        });
        return app;
    }

    internal static void ConfigureScimRoutes(IRouteBuilder routeBuilder, bool usePrefix, List<string> additionalRoutes = null)
    {
        if (additionalRoutes != null)
        {
            foreach (var additionalRoute in additionalRoutes)
            {
                routeBuilder.UseStandardScimEdp(additionalRoute, usePrefix);
            }
        }

        routeBuilder.ScimMapControllerRoute("getResourceTypes",
            pattern: SCIMEndpoints.ResourceType,
            defaults: new { controller = "ResourceTypes", action = "GetAll" });
        routeBuilder.ScimMapControllerRoute("getResourceType",
            pattern: SCIMEndpoints.ResourceType + "/{id}",
            defaults: new { controller = "ResourceTypes", action = "Get" });

        routeBuilder.ScimMapControllerRoute("getSchemas",
            pattern: SCIMEndpoints.Schemas,
            defaults: new { controller = "Schemas", action = "GetAll" });
        routeBuilder.ScimMapControllerRoute("getSchema",
            pattern: SCIMEndpoints.Schemas + "/{id}",
            defaults: new { controller = "Schemas", action = "Get" });

        routeBuilder.ScimMapControllerRoute("getServiceProviderConfig",
            pattern: SCIMEndpoints.ServiceProviderConfig,
            defaults: new { controller = "ServiceProviderConfig", action = "Get" });
        routeBuilder.ScimMapControllerRoute("bulk",
            pattern: $"{(usePrefix ? "{prefix}/" : string.Empty)}{SCIMEndpoints.Bulk}",
            defaults: new { controller = "Bulk", action = "Index" });

        routeBuilder.UseStandardScimEdp(SCIMEndpoints.User, usePrefix);
        routeBuilder.UseStandardScimEdp(SCIMEndpoints.Group, usePrefix);

        routeBuilder.MapRoute("catchAllGet",
            template: "{*url}",
            defaults: new { controller = "Default", action = "Get" });
        routeBuilder.MapRoute("catchAllPut",
            template: "{*url}",
            defaults: new { controller = "Default", action = "Put" });
        routeBuilder.MapRoute("catchAllPost",
            template: "{*url}",
            defaults: new { controller = "Default", action = "Post" });
        routeBuilder.MapRoute("catchAllDelete",
            template: "{*url}",
            defaults: new { controller = "Default", action = "Delete" });
        routeBuilder.MapRoute("catchAllPatch",
            template: "{*url}",
            defaults: new { controller = "Default", action = "Patch" });
    }
}
