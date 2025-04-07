// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseScim(this WebApplication webApp)
    {
        webApp.UseAuthentication();
        webApp.UseAuthorization();
        var usePrefix = webApp.Services.GetRequiredService<ScimHostOptions>().EnableRealm;
        webApp.UseMvc(o =>
        {
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

            webApp.UseStandardScimEdp(SCIMEndpoints.User, usePrefix);
            webApp.UseStandardScimEdp(SCIMEndpoints.Group, usePrefix);

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

    public static IRouteBuilder UseStandardScimEdp(this IRouteBuilder webApp, string controllerName, bool usePrefix)
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