// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed;

namespace Microsoft.AspNetCore.Builder;

public static class FastFedWebApplicationBuilderExtensions
{
    public static FastFedWebApplicationBuilder UseApplicationProvider(this FastFedWebApplicationBuilder builder)
    {
        builder.WebApplication.MapControllerRoute(name: "",
            pattern: $"{RouteNames.FastFed}/resolve",
            defaults: new { controller = "FastFed", action = "Resolve" });
        builder.WebApplication.MapControllerRoute(name: "",
            pattern: $"{RouteNames.FastFed}/whitelist",
            defaults: new { controller = "FastFed", action = "Whitelist" });
        builder.WebApplication.MapControllerRoute(name: "",
            pattern: RouteNames.Register,
            defaults: new { controller = "FastFed", action = "Register" });
        return builder;
    }
}
