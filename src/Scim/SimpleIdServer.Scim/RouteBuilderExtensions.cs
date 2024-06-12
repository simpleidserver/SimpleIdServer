// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;

namespace Microsoft.AspNetCore.Builder;

public static class RouteBuilderExtensions
{
    public static WebApplication UseScim(this WebApplication webApp, bool usePrefix = false)
    {
        webApp.ScimMapControllerRoute("getResourceTypes",
            pattern: SCIMEndpoints.ResourceType,
            defaults: new { controller = "ResourceTypes", action = "GetAll" });
        webApp.ScimMapControllerRoute("getResourceType",
            pattern: SCIMEndpoints.ResourceType + "/{id}",
            defaults: new { controller = "ResourceTypes", action = "Get" });

        webApp.ScimMapControllerRoute("getSchemas",
            pattern: SCIMEndpoints.Schemas,
            defaults: new { controller = "Schemas", action = "GetAll" });
        webApp.ScimMapControllerRoute("getSchema",
            pattern: SCIMEndpoints.Schemas + "/{id}",
            defaults: new { controller = "Schema", action = "Get" });

        webApp.ScimMapControllerRoute("getServiceProviderConfig",
            pattern: SCIMEndpoints.ServiceProviderConfig,
            defaults: new { controller = "ServiceProviderConfig", action = "Get" });

        webApp.ScimMapControllerRoute("bulk",
            pattern: $"{(usePrefix ? "{prefix}/" : string.Empty)}{SCIMEndpoints.Bulk}",
            defaults: new { controller = "Bulk", action = "Index" });

        webApp.UseStandardScimEdp(SCIMEndpoints.User, usePrefix);
        webApp.UseStandardScimEdp(SCIMEndpoints.Group, usePrefix);

        webApp.MapControllerRoute("catchAllGet",
            pattern: "{*url}",
            defaults: new { controller = "Default", action = "Get" });
        webApp.MapControllerRoute("catchAllPut",
            pattern: "{*url}",
            defaults: new { controller = "Default", action = "Put" });
        webApp.MapControllerRoute("catchAllPost",
            pattern: "{*url}",
            defaults: new { controller = "Default", action = "Post" });
        webApp.MapControllerRoute("catchAllDelete",
            pattern: "{*url}",
            defaults: new { controller = "Default", action = "Delete" });
        webApp.MapControllerRoute("catchAllPatch",
            pattern: "{*url}",
            defaults: new { controller = "Default", action = "Patch" });
        return webApp;
    }

    public static WebApplication UseStandardScimEdp(this WebApplication webApp, string controllerName, bool usePrefix)
    {
        webApp.ScimMapControllerRoute($"get{controllerName}",
            pattern: $"{(usePrefix ? "{prefix}/" : string.Empty)}{controllerName}",
            defaults: new { controller = controllerName, action = "GetAll" });
        webApp.ScimMapControllerRoute($"search{controllerName}",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/.search",
            defaults: new { controller = controllerName, action = "Search" });
        webApp.ScimMapControllerRoute($"getUnique{controllerName}",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Get" });
        webApp.ScimMapControllerRoute($"add{controllerName}",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + controllerName,
            defaults: new { controller = controllerName, action = "Add" });
        webApp.ScimMapControllerRoute($"delete{controllerName}",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Delete" });
        webApp.ScimMapControllerRoute($"update{controllerName}",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Update" });
        webApp.ScimMapControllerRoute($"patch{controllerName}",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + controllerName + "/{id}",
            defaults: new { controller = controllerName, action = "Patch" });
        return webApp;
    }
}