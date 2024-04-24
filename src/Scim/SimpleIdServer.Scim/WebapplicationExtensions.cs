// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

        webApplication.MapControllerRoute("getUsers",
            pattern: $"{(usePrefix ? "{prefix}/" : string.Empty)}{SCIMEndpoints.User}",
            defaults: new { controller = "Users", action = "GetAll" });
        webApplication.MapControllerRoute("searchUsers",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.User + "/.search",
            defaults: new { controller = "Users", action = "Search" });
        webApplication.MapControllerRoute("getUser",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.User + "/{id}",
            defaults: new { controller = "Users", action = "Get" });
        webApplication.MapControllerRoute("addUser",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.User,
            defaults: new { controller = "Users", action = "Add" });
        webApplication.MapControllerRoute("deleteUser",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.User + "/{id}",
            defaults: new { controller = "Users", action = "Delete" });
        webApplication.MapControllerRoute("updateUser",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.User + "/{id}",
            defaults: new { controller = "Users", action = "Update" });
        webApplication.MapControllerRoute("patchUser",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.User + "/{id}",
            defaults: new { controller = "Users", action = "Patch" });

        webApplication.MapControllerRoute("getGroups",
            pattern: $"{(usePrefix ? "{prefix}/" : string.Empty)}{SCIMEndpoints.Group}",
            defaults: new { controller = "Groups", action = "GetAll" });
        webApplication.MapControllerRoute("searchGroups",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.Group + "/.search",
            defaults: new { controller = "Groups", action = "Search" });
        webApplication.MapControllerRoute("getGroup",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.Group + "/{id}",
            defaults: new { controller = "Groups", action = "Get" });
        webApplication.MapControllerRoute("addGroup",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.Group,
            defaults: new { controller = "Groups", action = "Add" });
        webApplication.MapControllerRoute("deleteGroup",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.Group + "/{id}",
            defaults: new { controller = "Groups", action = "Delete" });
        webApplication.MapControllerRoute("updateGroup",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.Group + "/{id}",
            defaults: new { controller = "Groups", action = "Update" });
        webApplication.MapControllerRoute("patchGroup",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + SCIMEndpoints.Group + "/{id}",
            defaults: new { controller = "Groups", action = "Patch" });

        return webApplication;
    }
}