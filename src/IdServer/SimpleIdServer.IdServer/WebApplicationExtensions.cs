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
        public static WebApplication UseSID(this WebApplication webApplication, bool cookiesAlwaysSecure = true)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            webApplication.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = Http.CookieSecurePolicy.Always
            });
            webApplication.UseStaticFiles();
            webApplication.UseAuthentication();
            webApplication.UseAuthorization();

            var usePrefix = opts.UseRealm;
            webApplication.MapControllerRoute("oauthConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OAuthConfiguration,
                defaults: new { controller = "OAuthConfiguration", action = "Get" });

            webApplication.MapControllerRoute("openidConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OpenIDConfiguration,
                defaults: new { controller = "OpenIdConfiguration", action = "Get" });

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

            webApplication.MapControllerRoute("userInfoGet",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UserInfo,
                defaults: new { controller = "UserInfo", action = "Get" });
            webApplication.MapControllerRoute("userInfoPost",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UserInfo,
                defaults: new { controller = "UserInfo", action = "Post" });

            if (opts.MtlsEnabled)
            {
                webApplication.UseMiddleware<MtlsAuthenticationMiddleware>();
                webApplication.MapControllerRoute("tokenMtls",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.MtlsPrefix + "/" + Constants.EndPoints.Token,
                    defaults: new { controller = "Token", action = "Post" });
                webApplication.MapControllerRoute("tokenRevokeMtls",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.MtlsPrefix + "/" + Constants.EndPoints.TokenRevoke,
                    defaults: new { controller = "Token", action = "Revoke" });

                if(opts.IsBCEnabled)
                {
                    webApplication.MapControllerRoute("bcAuthorizeMtls",
                        pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.MtlsBCAuthorize,
                        defaults: new { controller = "BCAuthorize", action = "Post" });
                }
            }

            webApplication.MapControllerRoute("bcAuthorize",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BCAuthorize,
                defaults: new { controller = "BCAuthorize", action = "Post" });

            webApplication.MapControllerRoute("bcCallback",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BCCallback,
                defaults: new { controller = "BCCallback", action = "Post" });

            webApplication.MapControllerRoute("getGrant",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Grants + "/{id}",
                defaults: new { controller = "Grants", action = "Get" });
            webApplication.MapControllerRoute("revokeGrant",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Grants + "/{id}",
                defaults: new { controller = "Grants", action = "Revoke" });

            webApplication.MapControllerRoute(
                name: "defaultWithArea",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            webApplication.MapControllerRoute(
                name: "default",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + "{controller=Home}/{action=Index}/{id?}");

            return webApplication;
        }
    }
}
