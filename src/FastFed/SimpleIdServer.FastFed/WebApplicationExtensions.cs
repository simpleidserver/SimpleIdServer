// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static FastFedWebApplicationBuilder UseFastFed(this WebApplication webApplication)
    {
        webApplication.MapControllerRoute(name: "",
            pattern: RouteNames.ProviderMetadata,
            defaults: new { controller = "FastFedMetadata", action = "Get" });
        return new FastFedWebApplicationBuilder(webApplication);
    }
}
