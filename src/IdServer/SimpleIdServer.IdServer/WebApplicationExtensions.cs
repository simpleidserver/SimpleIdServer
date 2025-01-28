// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Infastructures;
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
            webApplication.UseMiddleware<LanguageMiddleware>();
            webApplication.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = Http.CookieSecurePolicy.Always
            });
            webApplication.UseStaticFiles();
            webApplication.UseAuthentication();
            webApplication.UseAuthorization();

            webApplication.SidMapControllerRoute("oauthConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OAuthConfiguration,
                defaults: new { controller = "OAuthConfiguration", action = "Get" });

            webApplication.SidMapControllerRoute("openidConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.OpenIDConfiguration,
                defaults: new { controller = "OpenIdConfiguration", action = "Get" });

            webApplication.SidMapControllerRoute("idServerConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdServerConfiguration,
                defaults: new { controller = "IdServerConfiguration", action = "Get" });

            webApplication.SidMapControllerRoute("jwks",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Jwks,
                defaults: new { controller = "Jwks", action = "Get" });

            webApplication.SidMapControllerRoute("authorization",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Authorization,
                defaults: new { controller = "Authorization", action = "Get" });

            webApplication.SidMapControllerRoute("authorizationCallback",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthorizationCallback,
                defaults: new { controller = "Authorization", action = "Callback" });

            webApplication.SidMapControllerRoute("token",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Token,
                defaults: new { controller = "Token", action = "Post" });
            webApplication.SidMapControllerRoute("tokenRevoke",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.TokenRevoke,
                defaults: new { controller = "Token", action = "Revoke" });

            webApplication.SidMapControllerRoute("publishedAuthorization",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.PushedAuthorizationRequest,
                defaults: new { controller = "PushedAuthorization", action = "Post" });

            webApplication.SidMapControllerRoute("tokenInfo",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.TokenInfo,
                defaults: new { controller = "TokenIntrospection", action = "Introspect" });

            webApplication.SidMapControllerRoute("registerClientAdd",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration,
                defaults: new { controller = "Registration", action = "Add" });
            webApplication.SidMapControllerRoute("getRegistrationForms",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/forms",
                defaults: new { controller = "Registration", action = "GetForms" });
            webApplication.SidMapControllerRoute("registerClientGet",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/{id?}",
                defaults: new { controller = "Registration", action = "Get" });
            webApplication.SidMapControllerRoute("registerClientDelete",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/{id?}",
                defaults: new { controller = "Registration", action = "Delete" });
            webApplication.SidMapControllerRoute("registerClientUpdate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Registration + "/{id?}",
                defaults: new { controller = "Registration", action = "Update" });


            webApplication.SidMapControllerRoute("userInfoGet",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UserInfo,
                defaults: new { controller = "UserInfo", action = "Get" });
            webApplication.SidMapControllerRoute("userInfoPost",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UserInfo,
                defaults: new { controller = "UserInfo", action = "Post" });

            var reccuringJobManager = webApplication.Services.GetRequiredService<IRecurringJobManager>();
            // Occurs every 10 seconds.
            reccuringJobManager.AddOrUpdate<UserSessionJob>(nameof(UserSessionJob), j => webApplication.Services.GetRequiredService<UserSessionJob>().Execute(), "*/10 * * * * *");

            if (opts.MtlsEnabled)
            {
                webApplication.UseMiddleware<MtlsAuthenticationMiddleware>();
                webApplication.SidMapControllerRoute("tokenMtls",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.MtlsPrefix + "/" + Constants.EndPoints.Token,
                    defaults: new { controller = "Token", action = "Post" });
                webApplication.SidMapControllerRoute("tokenRevokeMtls",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.MtlsPrefix + "/" + Constants.EndPoints.TokenRevoke,
                    defaults: new { controller = "Token", action = "Revoke" });

                if (opts.IsBCEnabled)
                {
                    webApplication.SidMapControllerRoute("bcAuthorizeMtls",
                        pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.MtlsBCAuthorize,
                        defaults: new { controller = "BCAuthorize", action = "Post" });
                }
            }

            if (opts.IsBCEnabled)
            {
                // Occurs every 15 seconds.
                reccuringJobManager.AddOrUpdate<BCNotificationJob>(nameof(BCNotificationJob), j => webApplication.Services.GetRequiredService<BCNotificationJob>().Execute(), "*/15 * * * * *");
                webApplication.SidMapControllerRoute("bcAuthorize",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BCAuthorize,
                    defaults: new { controller = "BCAuthorize", action = "Post" });

                webApplication.SidMapControllerRoute("bcCallback",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.BCCallback,
                    defaults: new { controller = "BCCallback", action = "Post" });
            }

            if (opts.IsUMAEnabled)
            {
                webApplication.SidMapControllerRoute("umaConfiguration",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAConfiguration,
                    defaults: new { controller = "UMAConfiguration", action = "Get" });

                webApplication.SidMapControllerRoute("umaPermissionsAdd",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAPermissions,
                    defaults: new { controller = "UMAPermissions", action = "Add" });

                webApplication.SidMapControllerRoute("umaPermissionsAddMultiple",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAPermissions + "/bulk",
                    defaults: new { controller = "UMAPermissions", action = "Bulk" });

                webApplication.SidMapControllerRoute("umaResourcesGetAll",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources,
                    defaults: new { controller = "UMAResources", action = "Get" });
                webApplication.SidMapControllerRoute("umaResourcesGetOne",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}",
                    defaults: new { controller = "UMAResources", action = "GetOne" });
                webApplication.SidMapControllerRoute("umaResourcesAdd",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources,
                    defaults: new { controller = "UMAResources", action = "Add" });
                webApplication.SidMapControllerRoute("umaResourcesUpdate",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}",
                    defaults: new { controller = "UMAResources", action = "Update" });
                webApplication.SidMapControllerRoute("umaResourcesDelete",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}",
                    defaults: new { controller = "UMAResources", action = "Delete" });
                webApplication.SidMapControllerRoute("umaResourcesAddPermissions",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}/permissions",
                    defaults: new { controller = "UMAResources", action = "AddPermissions" });
                webApplication.SidMapControllerRoute("umaResourcesGetPermissions",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}/permissions",
                    defaults: new { controller = "UMAResources", action = "GetPermissions" });
                webApplication.SidMapControllerRoute("umaResourcesDeletePermissions",
                    pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.UMAResources + "/{id}/permissions",
                    defaults: new { controller = "UMAResources", action = "DeletePermissions" });
            }

            webApplication.SidMapControllerRoute("relaunchErrorMessage",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ErrorMessages + "/{id}/relaunch",
                defaults: new { controller = "ErrorMessages", action = "Relaunch" });
            webApplication.SidMapControllerRoute("relaunchErrorMessagesByExternalid",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ErrorMessages + "/relaunch",
                defaults: new { controller = "ErrorMessages", action = "RelaunchAllByExternalId" });

            webApplication.SidMapControllerRoute("getGrant",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Grants + "/{id}",
                defaults: new { controller = "Grants", action = "Get" });
            webApplication.SidMapControllerRoute("revokeGrant",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Grants + "/{id}",
                defaults: new { controller = "Grants", action = "Revoke" });

            webApplication.SidMapControllerRoute("checkSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CheckSession,
                defaults: new { controller = "CheckSession", action = "Index" });
            webApplication.SidMapControllerRoute("endSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.EndSession,
                defaults: new { controller = "CheckSession", action = "EndSession" });
            webApplication.SidMapControllerRoute("endSessionCallback",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.EndSessionCallback,
                defaults: new { controller = "CheckSession", action = "EndSessionCallback" });
            webApplication.SidMapControllerRoute("activeSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ActiveSession,
                defaults: new { controller = "CheckSession", action = "IsActive" });

            webApplication.SidMapControllerRoute("form",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Form,
                defaults: new { controller = "Form", action = "Index" });

            webApplication.SidMapControllerRoute("searchUsers",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/.search",
                defaults: new { controller = "Users", action = "Search" });
            webApplication.SidMapControllerRoute("searchUserSessions",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/sessions/.search",
                defaults: new { controller = "Users", action = "SearchSessions" });
            webApplication.SidMapControllerRoute("getUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Get" });
            webApplication.SidMapControllerRoute("resolveUserRoles",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/roles",
                defaults: new { controller = "Users", action = "ResolveRoles" });
            webApplication.SidMapControllerRoute("addUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users,
                defaults: new { controller = "Users", action = "Add" });
            webApplication.SidMapControllerRoute("updateUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Update" });
            webApplication.SidMapControllerRoute("deleteUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Delete" });
            webApplication.SidMapControllerRoute("addUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials",
                defaults: new { controller = "Users", action = "AddCredential" });
            webApplication.SidMapControllerRoute("updateUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials/{credentialId}",
                defaults: new { controller = "Users", action = "UpdateCredential" });
            webApplication.SidMapControllerRoute("deleteUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials/{credentialId}",
                defaults: new { controller = "Users", action = "DeleteCredential" });
            webApplication.SidMapControllerRoute("setDefaultUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials/{credentialId}/default",
                defaults: new { controller = "Users", action = "DefaultCredential" });
            webApplication.SidMapControllerRoute("updateUserClaims",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/claims",
                defaults: new { controller = "Users", action = "UpdateClaims" });
            webApplication.SidMapControllerRoute("addUserGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/groups/{groupId}",
                defaults: new { controller = "Users", action = "AddGroup" });
            webApplication.SidMapControllerRoute("deleteUserGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/groups/{groupId}",
                defaults: new { controller = "Users", action = "RemoveGroup" });
            webApplication.SidMapControllerRoute("revokeUserConsent",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/consents/{consentId}",
                defaults: new { controller = "Users", action = "RevokeConsent" });
            webApplication.SidMapControllerRoute("revokeUserSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/sessions/{sessionId}",
                defaults: new { controller = "Users", action = "RevokeSession" });
            webApplication.SidMapControllerRoute("revokeUserSessions",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/sessions",
                defaults: new { controller = "Users", action = "RevokeSessions" });
            webApplication.SidMapControllerRoute("unlinkUserExternalAuthProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/authproviders/unlink",
                defaults: new { controller = "Users", action = "UnlinkExternalAuthProvider" });
            webApplication.SidMapControllerRoute("generateDecentralizedIdentity",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/did",
                defaults: new { controller = "Users", action = "GenerateDecentralizedIdentity" });
            webApplication.SidMapControllerRoute("getPicture",
                pattern: Constants.EndPoints.Users + "/{id}/picture",
                defaults: new { controller = "Users", action = "GetPicture" });
            webApplication.SidMapControllerRoute("updatePicture",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/picture",
                defaults: new { controller = "Users", action = "UpdatePicture" });


            webApplication.SidMapControllerRoute("searchIdProvisioning",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/.search",
                defaults: new { controller = "IdentityProvisioning", action = "Search" });
            webApplication.SidMapControllerRoute("importRepresentations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/{processId}/import",
                defaults: new { controller = "IdentityProvisioning", action = "Import" });
            webApplication.SidMapControllerRoute("getIdProvisioning",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}",
                defaults: new { controller = "IdentityProvisioning", action = "Get" });
            webApplication.SidMapControllerRoute("updateIdProvisioningDetails",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/details",
                defaults: new { controller = "IdentityProvisioning", action = "UpdateDetails" });
            webApplication.SidMapControllerRoute("updateIdProvisioningProperties",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/values",
                defaults: new { controller = "IdentityProvisioning", action = "UpdateProperties" });
            webApplication.SidMapControllerRoute("removeIdProvisioningMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "IdentityProvisioning", action = "RemoveMapper" });
            webApplication.SidMapControllerRoute("updateIdProvisioningMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "IdentityProvisioning", action = "UpdateMapper" });
            webApplication.SidMapControllerRoute("getIdProvisioningMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "IdentityProvisioning", action = "GetMapper" });
            webApplication.SidMapControllerRoute("addIdProvisioningMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/mappers",
                defaults: new { controller = "IdentityProvisioning", action = "AddMapper" });
            webApplication.SidMapControllerRoute("extractRepresentations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/extract",
                defaults: new { controller = "IdentityProvisioning", action = "Extract" });
            webApplication.SidMapControllerRoute("idProvisioningTestConnection",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/test",
                defaults: new { controller = "IdentityProvisioning", action = "TestConnection" });
            webApplication.SidMapControllerRoute("idProvisioningAllowedAttributes",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/allowedattributes",
                defaults: new { controller = "IdentityProvisioning", action = "GetAllowedAttributes" });
            webApplication.SidMapControllerRoute("searchIdProvisioningImport",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/import/.search",
                defaults: new { controller = "IdentityProvisioning", action = "SearchImport" });


            webApplication.SidMapControllerRoute("getAllNetworks",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks,
                defaults: new { controller = "Networks", action = "GetAll" });
            webApplication.SidMapControllerRoute("removeNetwork",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks + "/{name}",
                defaults: new { controller = "Networks", action = "Remove" });
            webApplication.SidMapControllerRoute("addNetwork",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks,
                defaults: new { controller = "Networks", action = "Add" });
            webApplication.SidMapControllerRoute("deployContract",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Networks + "/{name}/deploy",
                defaults: new { controller = "Networks", action = "Deploy" });

            webApplication.SidMapControllerRoute("deviceAuthorization",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.DeviceAuthorization,
                defaults: new { controller = "DeviceAuthorization", action = "Post" });

            webApplication.SidMapControllerRoute("getAllAmrs",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences,
                defaults: new { controller = "AuthenticationClassReferences", action = "GetAll" });
            webApplication.SidMapControllerRoute("addAmr",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences,
                defaults: new { controller = "AuthenticationClassReferences", action = "Add" });
            webApplication.SidMapControllerRoute("getAllAuthForms",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences + "/forms",
                defaults: new { controller = "AuthenticationClassReferences", action = "GetAllForms" });
            webApplication.SidMapControllerRoute("deleteAmr",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences + "/{id}",
                defaults: new { controller = "AuthenticationClassReferences", action = "Delete" });
            webApplication.SidMapControllerRoute("assignRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationClassReferences + "/{id}/assign",
                defaults: new { controller = "AuthenticationClassReferences", action = "AssignRegistrationWorkflow" });


            webApplication.SidMapControllerRoute("searchIdProviders",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/.search",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Search" });
            webApplication.SidMapControllerRoute("getIdProviderDefinitions",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/defs",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "GetDefinitions" });
            webApplication.SidMapControllerRoute("removeIdProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Remove" });
            webApplication.SidMapControllerRoute("getIdProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Get" });
            webApplication.SidMapControllerRoute("addIdProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders,
                defaults: new { controller = "AuthenticationSchemeProviders", action = "Add" });
            webApplication.SidMapControllerRoute("updateIdProviderDetails",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/details",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "UpdateDetails" });
            webApplication.SidMapControllerRoute("updateIdProviderValues",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/values",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "UpdateValues" });
            webApplication.SidMapControllerRoute("addIdProviderMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/mappers",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "AddMapper" });
            webApplication.SidMapControllerRoute("removeIdProviderMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "RemoveMapper" });
            webApplication.SidMapControllerRoute("updateIdProviderMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "UpdateMapper" });

            webApplication.SidMapControllerRoute("getFidoConfiguration",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.FidoConfiguration,
                defaults: new { controller = "FidoConfiguration", action = "Index" });

            webApplication.SidMapControllerRoute("GetUserLockingOptions",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods + "/userlockingoptions",
                defaults: new { controller = "AuthenticationMethods", action = "GetUserLockingOptions" });
            webApplication.SidMapControllerRoute("UpdateUserLockingOptions",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods + "/userlockingoptions",
                defaults: new { controller = "AuthenticationMethods", action = "UpdateUserLockingOptions" });
            webApplication.SidMapControllerRoute("getAllAuthMethods",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods,
                defaults: new { controller = "AuthenticationMethods", action = "GetAll" });
            webApplication.SidMapControllerRoute("updateAuthMethodConfigurations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods + "/{amr}",
                defaults: new { controller = "AuthenticationMethods", action = "Update" });
            webApplication.SidMapControllerRoute("getAuthMethodConfigurations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods + "/{amr}",
                defaults: new { controller = "AuthenticationMethods", action = "Get" });

            webApplication.SidMapControllerRoute("getAllRegistrationWorkflows",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows,
                defaults: new { controller = "RegistrationWorkflows", action = "GetAll" });
            webApplication.SidMapControllerRoute("getRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows + "/{id}",
                defaults: new { controller = "RegistrationWorkflows", action = "Get" });
            webApplication.SidMapControllerRoute("deleteRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows + "/{id}",
                defaults: new { controller = "RegistrationWorkflows", action = "Delete" });
            webApplication.SidMapControllerRoute("addRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows,
                defaults: new { controller = "RegistrationWorkflows", action = "Add" });
            webApplication.SidMapControllerRoute("updateRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows + "/{id}",
                defaults: new { controller = "RegistrationWorkflows", action = "Update" });

            webApplication.SidMapControllerRoute("addApiResource",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ApiResources,
                defaults: new { controller = "ApiResources", action = "Add" });
            webApplication.SidMapControllerRoute("searchApiResource",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ApiResources + "/.search",
                defaults: new { controller = "ApiResources", action = "Search" });
            webApplication.SidMapControllerRoute("deleteApiResource",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ApiResources + "/{id}",
                defaults: new { controller = "ApiResources", action = "Delete" });

            webApplication.SidMapControllerRoute("searchAuditing",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Auditing + "/.search",
                defaults: new { controller = "Auditing", action = "Search" });

            webApplication.SidMapControllerRoute("getAllRealmScopes",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/realmscopes",
                defaults: new { controller = "Scopes", action = "GetAllRealmScopes" });
            webApplication.SidMapControllerRoute("searchScopes",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/.search",
                defaults: new { controller = "Scopes", action = "Search" });
            webApplication.SidMapControllerRoute("getScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}",
                defaults: new { controller = "Scopes", action = "Get" });
            webApplication.SidMapControllerRoute("deleteScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}",
                defaults: new { controller = "Scopes", action = "Delete" });
            webApplication.SidMapControllerRoute("addScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes,
                defaults: new { controller = "Scopes", action = "Add" });
            webApplication.SidMapControllerRoute("updateScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}",
                defaults: new { controller = "Scopes", action = "Update" });
            webApplication.SidMapControllerRoute("addClaimMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/mappers",
                defaults: new { controller = "Scopes", action = "AddClaimMapper" });
            webApplication.SidMapControllerRoute("deleteClaimMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "Scopes", action = "RemoveClaimMapper" });
            webApplication.SidMapControllerRoute("updateClaimMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "Scopes", action = "UpdateClaimMapper" });
            webApplication.SidMapControllerRoute("updateScopeResources",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/resources",
                defaults: new { controller = "Scopes", action = "UpdateResources" });

            webApplication.SidMapControllerRoute("searchCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/.search",
                defaults: new { controller = "CertificateAuthorities", action = "Search" });
            webApplication.SidMapControllerRoute("generateCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/generate",
                defaults: new { controller = "CertificateAuthorities", action = "Generate" });
            webApplication.SidMapControllerRoute("importCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/import",
                defaults: new { controller = "CertificateAuthorities", action = "Import" });
            webApplication.SidMapControllerRoute("addCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities,
                defaults: new { controller = "CertificateAuthorities", action = "Add" });
            webApplication.SidMapControllerRoute("removeCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}",
                defaults: new { controller = "CertificateAuthorities", action = "Remove" });
            webApplication.SidMapControllerRoute("getCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}",
                defaults: new { controller = "CertificateAuthorities", action = "Get" });
            webApplication.SidMapControllerRoute("removeClientCertificate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}/clientcertificates/{clientCertificateId}",
                defaults: new { controller = "CertificateAuthorities", action = "RemoveClientCertificate" });
            webApplication.SidMapControllerRoute("addClientCertificate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}/clientcertificates",
                defaults: new { controller = "CertificateAuthorities", action = "AddClientCertificate" });

            webApplication.SidMapControllerRoute("searchClients",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/.search",
                defaults: new { controller = "Clients", action = "Search" });
            webApplication.SidMapControllerRoute("getAllClients",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients,
                defaults: new { controller = "Clients", action = "GetAll" });
            webApplication.SidMapControllerRoute("addClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients,
                defaults: new { controller = "Clients", action = "Add" });
            webApplication.SidMapControllerRoute("getClientByTechnicalId",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/bytechnicalid/{id}",
                defaults: new { controller = "Clients", action = "GetByTechnicalId" });
            webApplication.SidMapControllerRoute("getClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}",
                defaults: new { controller = "Clients", action = "Get" });
            webApplication.SidMapControllerRoute("deleteClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}",
                defaults: new { controller = "Clients", action = "Delete" });
            webApplication.SidMapControllerRoute("updateClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}",
                defaults: new { controller = "Clients", action = "Update" });
            webApplication.SidMapControllerRoute("updateAdvancedClientSettings",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/advanced",
                defaults: new { controller = "Clients", action = "UpdateAdvanced" });
            webApplication.SidMapControllerRoute("removeClientScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/scopes/{name}",
                defaults: new { controller = "Clients", action = "RemoveScope" });
            webApplication.SidMapControllerRoute("addClientScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/scopes",
                defaults: new { controller = "Clients", action = "AddScope" });
            webApplication.SidMapControllerRoute("generateSigKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/sigkey/generate",
                defaults: new { controller = "Clients", action = "GenerateSigKey" });
            webApplication.SidMapControllerRoute("generateEncKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/enckey/generate",
                defaults: new { controller = "Clients", action = "GenerateEncKey" });
            webApplication.SidMapControllerRoute("addSigKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/sigkey",
                defaults: new { controller = "Clients", action = "AddSigKey" });
            webApplication.SidMapControllerRoute("addEncKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/enckey",
                defaults: new { controller = "Clients", action = "AddEncKey" });
            webApplication.SidMapControllerRoute("removeClientKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/keys/{keyId}",
                defaults: new { controller = "Clients", action = "RemoveKey" });
            webApplication.SidMapControllerRoute("updateClientCredentials",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/credentials",
                defaults: new { controller = "Clients", action = "UpdateCredentials" });
            webApplication.SidMapControllerRoute("addClientRole",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/roles",
                defaults: new { controller = "Clients", action = "AddRole" });
            webApplication.SidMapControllerRoute("updateClientRealms",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/realms",
                defaults: new { controller = "Clients", action = "UpdateRealms" });

            webApplication.SidMapControllerRoute("searchGroups",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/.search",
                defaults: new { controller = "Groups", action = "Search" });
            webApplication.SidMapControllerRoute("deleteGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/delete",
                defaults: new { controller = "Groups", action = "Delete" });
            webApplication.SidMapControllerRoute("getGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}",
                defaults: new { controller = "Groups", action = "Get" });
            webApplication.SidMapControllerRoute("getGroupHierarchy",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}/hierarchy",
                defaults: new { controller = "Groups", action = "GetHierarchicalGroup" });
            webApplication.SidMapControllerRoute("addGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups,
                defaults: new { controller = "Groups", action = "Add" });
            webApplication.SidMapControllerRoute("addGroupRole",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}/roles",
                defaults: new { controller = "Groups", action = "AddRole" });
            webApplication.SidMapControllerRoute("removeGroupRole",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}/roles/{roleId}",
                defaults: new { controller = "Groups", action = "RemoveRole" });

            webApplication.SidMapControllerRoute("getStats",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Statistics,
                defaults: new { controller = "Statistics", action = "Get" });

            webApplication.SidMapControllerRoute("getAllRealms",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Realms,
                defaults: new { controller = "Realms", action = "GetAll" });
            webApplication.SidMapControllerRoute("addRealm",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Realms,
                defaults: new { controller = "Realms", action = "Add" });
            webApplication.SidMapControllerRoute("deleteRealm",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Realms + "/{id}",
                defaults: new { controller = "Realms", action = "Delete" });

            webApplication.SidMapControllerRoute("getAllLanguages",
                pattern: Constants.EndPoints.Languages,
                defaults: new { controller = "Languages", action = "GetAll" });

            webApplication.MapControllerRoute(
                name: "defaultWithArea",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            webApplication.MapControllerRoute(
                name: "default",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + "{controller=Home}/{action=Index}/{id?}");

            if(usePrefix)
            {
                webApplication.MapControllerRoute(
                   name: "getSessions",
                   pattern: "{controller=Sessions}/{action=Index}"
                );
            }

            return webApplication;
        }
    }
}
