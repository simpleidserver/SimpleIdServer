// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseIdentityProvider(this WebApplication application)
    {
        application.MapControllerRoute(name: "",
            pattern: RouteNames.Start,
            defaults: new { controller = "FastFed", action = "Start" });
        application.MapControllerRoute(name: "",
            pattern:  RouteNames.ProviderMetadata,
            defaults: new { controller = "FastFed", action = "GetMetadata" });
        return application;
    }
}
