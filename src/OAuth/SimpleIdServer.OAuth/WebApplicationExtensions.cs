// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Middlewares;
using SimpleIdServer.OAuth.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseSID(this WebApplication webApplication, bool usePrefix = false)
        {
            webApplication.UseMiddleware<MtlsAuthenticationMiddleware>();
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
            webApplication.MapControllerRoute("tokenRevoke",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.TokenRevoke,
                defaults: new { controller = "Token", action = "Revoke" });
            webApplication.MapControllerRoute("tokenInfo",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.TokenInfo,
                defaults: new { controller = "TokenIntrospection", action = "Introspect" });
            webApplication.MapControllerRoute("managementClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ClientManagement,
                defaults: new { controller = "ClientManagement", action = "Add" });
            webApplication.MapControllerRoute("registerClientAdd",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration,
                defaults: new { controller = "Registration", action = "Add" });
            webApplication.MapControllerRoute("registerClientGet",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/{id?}",
                defaults: new { controller = "Registration", action = "Get" });
            webApplication.MapControllerRoute("registerClientDelete",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/{id?}",
                defaults: new { controller = "Registration", action = "Delete" });
            webApplication.MapControllerRoute("registerClientUpdate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/{id?}",
                defaults: new { controller = "Registration", action = "Update" });
            if (webApplication.Services.GetRequiredService<IOptions<OAuthHostOptions>>().Value.MtlsEnabled)
            {
                webApplication.MapControllerRoute("tokenMtls",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + "mtls/" + Constants.EndPoints.Token,
                    defaults: new { controller = "Token", action = "Post" });
                webApplication.MapControllerRoute("tokenRevokeMtls",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + "mtls/" + Constants.EndPoints.TokenRevoke,
                    defaults: new { controller = "Token", action = "Revoke" });
            }

            return webApplication;
        }
    }
}
