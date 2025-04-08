// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Infrastructure;

namespace Microsoft.AspNetCore.Routing;

public static class RouterBuilderExtensions
{
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
