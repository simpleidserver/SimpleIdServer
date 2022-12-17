// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseSID(this WebApplication webApplication, bool usePrefix = false)
        {
            webApplication.MapControllerRoute("configuration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OAuthConfiguration,
                defaults: new { controller = "Configuration", action = "Get" });
            webApplication.MapControllerRoute("jwks",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Jwks,
                defaults: new { controller = "Jwks", action = "Get" });
            webApplication.MapControllerRoute("authorization",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Authorization,
                defaults: new { controller = "Authorization", action = "Get" });
            webApplication.MapControllerRoute("token",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Token,
                defaults: new { controller = "Token", action = "Post" });
            return webApplication;
        }
    }
}
