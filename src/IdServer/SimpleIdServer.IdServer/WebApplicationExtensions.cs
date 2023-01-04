// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Middlewares;
using SimpleIdServer.IdServer.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseSID(this WebApplication webApplication)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            var usePrefix = opts.UseRealm;
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


            webApplication.MapControllerRoute("authSchemeProviderGet",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthSchemeProviders + "/{id}",
                defaults: new { controller = "AuthSchemeProviders", action = "Get" });
            webApplication.MapControllerRoute("authSchemeProviderGetAll",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthSchemeProviders,
                defaults: new { controller = "AuthSchemeProviders", action = "GetAll" });
            webApplication.MapControllerRoute("authSchemeProviderEnable",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthSchemeProviders + "/{id}/enable",
                defaults: new { controller = "AuthSchemeProviders", action = "Enable" });
            webApplication.MapControllerRoute("authSchemeProviderDisable",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthSchemeProviders + "/{id}/disable",
                defaults: new { controller = "AuthSchemeProviders", action = "Disable" });
            webApplication.MapControllerRoute("authSchemeProviderUpdate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthSchemeProviders + "/{id}",
                defaults: new { controller = "AuthSchemeProviders", action = "UpdateOptions" });

            if (opts.MtlsEnabled)
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
