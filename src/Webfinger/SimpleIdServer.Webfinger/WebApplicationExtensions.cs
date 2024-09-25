// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Webfinger.Client;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static WebApplication UseWebfinger(this WebApplication webApplication)
    {
        webApplication.MapControllerRoute("getWebfinger",
                        pattern: RouteNames.WellKnownWebFinger,
                        defaults: new { controller = "Webfinger", action = "Get" });
        return webApplication;
    }
}
