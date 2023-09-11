// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Middlewares;
using SimpleIdServer.IdServer.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseSID(this WebApplication webApplication, bool cookiesAlwaysSecure = true)
        {
            var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
            var usePrefix = opts.UseRealm;
            if(usePrefix) webApplication.UseMiddleware<RealmMiddleware>();
            webApplication.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = Http.CookieSecurePolicy.Always
            });
            webApplication.UseStaticFiles();
            webApplication.UseAuthentication();
            webApplication.UseAuthorization();

            webApplication.MapControllerRoute("oauthConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OAuthConfiguration,
                defaults: new { controller = "OAuthConfiguration", action = "Get" });

            webApplication.MapControllerRoute("openidConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OpenIDConfiguration,
                defaults: new { controller = "OpenIdConfiguration", action = "Get" });

            webApplication.MapControllerRoute("idServerConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdServerConfiguration,
                defaults: new { controller = "IdServerConfiguration", action = "Get" });

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

            webApplication.MapControllerRoute("publishedAuthorization",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.PushedAuthorizationRequest,
                defaults: new { controller = "PushedAuthorization", action = "Post" });

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

            if(opts.IsBCEnabled)
            {
                var reccuringJobManager = webApplication.Services.GetRequiredService<IRecurringJobManager>();
                // Occurs every 15 seconds.
                reccuringJobManager.AddOrUpdate<BCNotificationJob>(nameof(BCNotificationJob), j => webApplication.Services.GetRequiredService<BCNotificationJob>().Execute(), "*/15 * * * * *");
                webApplication.MapControllerRoute("bcAuthorize",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BCAuthorize,
                    defaults: new { controller = "BCAuthorize", action = "Post" });

                webApplication.MapControllerRoute("bcCallback",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BCCallback,
                    defaults: new { controller = "BCCallback", action = "Post" });
            }

            if(opts.IsUMAEnabled)
            {
                webApplication.MapControllerRoute("umaConfiguration",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAConfiguration,
                    defaults: new { controller = "UMAConfiguration", action = "Get" });

                webApplication.MapControllerRoute("umaPermissionsAddOne",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAPermissions,
                    defaults: new { controller = "UMAPermissions", action = "Add" });
                webApplication.MapControllerRoute("umaPermissionsAddMultiple",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAPermissions,
                    defaults: new { controller = "UMAPermissions", action = "AddList" });

                webApplication.MapControllerRoute("umaResourcesGetAll",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources,
                    defaults: new { controller = "UMAResources", action = "Get" });
                webApplication.MapControllerRoute("umaResourcesGetOne",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}",
                    defaults: new { controller = "UMAResources", action = "GetOne" });
                webApplication.MapControllerRoute("umaResourcesAdd",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources,
                    defaults: new { controller = "UMAResources", action = "Add" });
                webApplication.MapControllerRoute("umaResourcesUpdate",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}",
                    defaults: new { controller = "UMAResources", action = "Update" });
                webApplication.MapControllerRoute("umaResourcesDelete",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}",
                    defaults: new { controller = "UMAResources", action = "Delete" });
                webApplication.MapControllerRoute("umaResourcesAddPermissions",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}/permissions",
                    defaults: new { controller = "UMAResources", action = "AddPermissions" });
                webApplication.MapControllerRoute("umaResourcesGetPermissions",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}/permissions",
                    defaults: new { controller = "UMAResources", action = "GetPermissions" });
                webApplication.MapControllerRoute("umaResourcesDeletePermissions",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}/permissions",
                    defaults: new { controller = "UMAResources", action = "DeletePermissions" });
            }

            webApplication.MapControllerRoute("getGrant",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Grants + "/{id}",
                defaults: new { controller = "Grants", action = "Get" });
            webApplication.MapControllerRoute("revokeGrant",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Grants + "/{id}",
                defaults: new { controller = "Grants", action = "Revoke" });

            webApplication.MapControllerRoute("checkSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CheckSession,
                defaults: new { controller = "CheckSession", action = "Index" });
            webApplication.MapControllerRoute("endSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.EndSession,
                defaults: new { controller = "CheckSession", action = "EndSession" });
            webApplication.MapControllerRoute("endSessionCallback",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.EndSessionCallback,
                defaults: new { controller = "CheckSession", action = "EndSessionCallback" });
            webApplication.MapControllerRoute("activeSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ActiveSession,
                defaults: new { controller = "CheckSession", action = "IsActive" });

            webApplication.MapControllerRoute("form",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Form,
                defaults: new { controller = "Form", action = "Index" });

            webApplication.MapControllerRoute("addUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users,
                defaults: new { controller = "Users", action = "Add" });
            webApplication.MapControllerRoute("getUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Get" });
            webApplication.MapControllerRoute("deleteUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Delete" });
            webApplication.MapControllerRoute("replaceCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials",
                defaults: new { controller = "Users", action = "ReplaceCredential" });
            webApplication.MapControllerRoute("generateDecentralizedIdentity",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/did",
                defaults: new { controller = "Users", action = "GenerateDecentralizedIdentity" });

            webApplication.MapControllerRoute("extractRepresentations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{name}/{id}/enqueue",
                defaults: new { controller = "IdentityProvisioning", action = "Enqueue" });
            webApplication.MapControllerRoute("importRepresentations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/import",
                defaults: new { controller = "IdentityProvisioning", action = "Import" });


            webApplication.MapControllerRoute("getAllNetworks",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks,
                defaults: new { controller = "Networks", action = "GetAll" });
            webApplication.MapControllerRoute("removeNetwork",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks + "/{name}",
                defaults: new { controller = "Networks", action = "Remove" });
            webApplication.MapControllerRoute("addNetwork",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks,
                defaults: new { controller = "Networks", action = "Add" });
            webApplication.MapControllerRoute("deployContract",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks + "/{name}/deploy",
                defaults: new { controller = "Networks", action = "Deploy" });

            webApplication.MapControllerRoute("deviceAuthorization",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.DeviceAuthorization,
                defaults: new { controller = "DeviceAuthorization", action = "Post" });

            webApplication.MapControllerRoute("getAllAmrs",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences,
                defaults: new { controller = "AuthenticationClassReferences", action = "GetAll" });
            webApplication.MapControllerRoute("addAmr",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences,
                defaults: new { controller = "AuthenticationClassReferences", action = "Add" });
            webApplication.MapControllerRoute("deleteAmr",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences + "/{id}",
                defaults: new { controller = "AuthenticationClassReferences", action = "Delete" });


            webApplication.MapControllerRoute("searchIdProviders",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/.search",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Search" });
            webApplication.MapControllerRoute("removeIdProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Remove" });
            webApplication.MapControllerRoute("getIdProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Get" });
            webApplication.MapControllerRoute("addIdProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders,
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Add" });
            webApplication.MapControllerRoute("updateIdProviderDetails",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/details",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "UpdateDetails" });
            webApplication.MapControllerRoute("updateIdProviderValues",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/values",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "UpdateValues" });
            webApplication.MapControllerRoute("addIdProviderMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/mappers",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "AddMapper" });
            webApplication.MapControllerRoute("removeIdProviderMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "RemoveMapper" });
            webApplication.MapControllerRoute("updateIdProviderMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "UpdateMapper" });

            webApplication.MapControllerRoute("getFidoConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.FidoConfiguration,
                defaults: new { controller = "FidoConfiguration", action = "Index" });

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
