// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed;

namespace Microsoft.AspNetCore.Builder;

public static class FastFedWebApplicationBuilderExtensions
{
    public static FastFedWebApplicationBuilder UseIdentityProvider(this FastFedWebApplicationBuilder builder)
    {
        builder.WebApplication.MapControllerRoute(name: "StartFastFed",
            pattern: RouteNames.Start,
            defaults: new { controller = "FastFed", action = "Start" });
        return builder;
    }
}
