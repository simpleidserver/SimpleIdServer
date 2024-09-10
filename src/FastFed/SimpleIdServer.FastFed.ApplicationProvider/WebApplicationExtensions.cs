// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplicationProvider(this WebApplication application)
    {
        application.MapControllerRoute(name: "",
            pattern:  RouteNames.ProviderMetadata,
            defaults: new { controller = "FastFed", action = "GetMetadata" });
        application.MapControllerRoute(name: "",
            pattern: $"{RouteNames.FastFed}/resolve",
            defaults: new { controller = "FastFed", action = "Resolve" });
        application.MapControllerRoute(name: "",
            pattern: $"{RouteNames.FastFed}/whitelist",
            defaults: new { controller = "FastFed", action = "Whitelist" });
        application.MapControllerRoute(name: "",
            pattern: RouteNames.Register,
            defaults: new { controller = "FastFed", action = "Register" });

        application.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        return application;
    }
}
