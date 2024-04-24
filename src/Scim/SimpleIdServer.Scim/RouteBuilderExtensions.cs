// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SimpleIdServer.Scim.Domains;

namespace Microsoft.AspNetCore.Builder;

public static class RouteBuilderExtensions
{
    public static IRouteBuilder UseScim(this IRouteBuilder routeBuilder, bool usePrefix = false)
    {
        routeBuilder.MapRoute("getResourceTypes",
            template: SCIMEndpoints.ResourceType,
            defaults: new { controller = "ResourceTypes", action = "GetAll" });
        routeBuilder.MapRoute("getResourceType",
            template: SCIMEndpoints.ResourceType + "/{id}",
            defaults: new { controller = "ResourceTypes", action = "Get" });

        routeBuilder.MapRoute("getSchemas",
            template: SCIMEndpoints.Schemas,
            defaults: new { controller = "Schemas", action = "GetAll" });
        routeBuilder.MapRoute("getSchema",
            template: SCIMEndpoints.Schemas + "/{id}",
            defaults: new { controller = "Schema", action = "Get" });

        routeBuilder.MapRoute("getServiceProviderConfig",
            template: SCIMEndpoints.ServiceProviderConfig,
            defaults: new { controller = "ServiceProviderConfig", action = "Get" });

        routeBuilder.MapRoute("bulk",
            template: $"{(usePrefix ? "{prefix}/" : string.Empty)}{SCIMEndpoints.Bulk}",
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
        return routeBuilder;
    }

    public static IRouteBuilder UseStandardScimEdp(this IRouteBuilder routeBuilder, string controllerName, bool usePrefix)
    {
        routeBuilder.MapRoute($"get{controllerName}",
            template: $"{(usePrefix ? "{prefix}/" : string.Empty)}{controllerName}",
            defaults: new { controller = controllerName, action = "GetAll" });
        routeBuilder.MapRoute($"search{controllerName}",
            template: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/.search",
            defaults: new { controller = controllerName, action = "Search" });
        routeBuilder.MapRoute($"getUnique{controllerName}",
            template: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Get" });
        routeBuilder.MapRoute($"add{controllerName}",
            template: (usePrefix ? "{prefix}/" : string.Empty) + controllerName,
            defaults: new { controller = controllerName, action = "Add" });
        routeBuilder.MapRoute($"delete{controllerName}",
            template: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Delete" });
        routeBuilder.MapRoute($"update{controllerName}",
            template: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Update" });
        routeBuilder.MapRoute($"patch{controllerName}",
            template: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Patch" });
        return routeBuilder;
    }
}