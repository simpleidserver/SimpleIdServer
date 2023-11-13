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

            webApplication.MapControllerRoute("searchUsers",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/.search",
                defaults: new { controller = "Users", action = "Search" });
            webApplication.MapControllerRoute("getUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Get" });
            webApplication.MapControllerRoute("resolveUserRoles",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/roles",
                defaults: new { controller = "Users", action = "ResolveRoles" });
            webApplication.MapControllerRoute("addUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users,
                defaults: new { controller = "Users", action = "Add" });
            webApplication.MapControllerRoute("updateUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Update" });
            webApplication.MapControllerRoute("deleteUser",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}",
                defaults: new { controller = "Users", action = "Delete" });
            webApplication.MapControllerRoute("addUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials",
                defaults: new { controller = "Users", action = "AddCredential" });
            webApplication.MapControllerRoute("updateUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials/{credentialId}",
                defaults: new { controller = "Users", action = "UpdateCredential" });
            webApplication.MapControllerRoute("deleteUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials/{credentialId}",
                defaults: new { controller = "Users", action = "DeleteCredential" });
            webApplication.MapControllerRoute("setDefaultUserCredential",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/credentials/{credentialId}/default",
                defaults: new { controller = "Users", action = "DefaultCredential" });
            webApplication.MapControllerRoute("updateUserClaims",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/claims",
                defaults: new { controller = "Users", action = "UpdateClaims" });
            webApplication.MapControllerRoute("addUserGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/groups/{groupId}",
                defaults: new { controller = "Users", action = "AddGroup" });
            webApplication.MapControllerRoute("deleteUserGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/groups/{groupId}",
                defaults: new { controller = "Users", action = "RemoveGroup" });
            webApplication.MapControllerRoute("revokeUserConsent",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/consents/{consentId}",
                defaults: new { controller = "Users", action = "RevokeConsent" });
            webApplication.MapControllerRoute("revokeUserSession",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/sessions/{sessionId}",
                defaults: new { controller = "Users", action = "RevokeSession" });
            webApplication.MapControllerRoute("unlinkUserExternalAuthProvider",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/authproviders/unlink",
                defaults: new { controller = "Users", action = "UnlinkExternalAuthProvider" });
            webApplication.MapControllerRoute("generateDecentralizedIdentity",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Users + "/{id}/did",
                defaults: new { controller = "Users", action = "GenerateDecentralizedIdentity" });

            webApplication.MapControllerRoute("searchIdProvisioning",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/.search",
                defaults: new { controller = "IdentityProvisioning", action = "Search" });
            webApplication.MapControllerRoute("importRepresentations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/import",
                defaults: new { controller = "IdentityProvisioning", action = "Import" });
            webApplication.MapControllerRoute("removeIdProvisioning",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}",
                defaults: new { controller = "IdentityProvisioning", action = "Remove" });
            webApplication.MapControllerRoute("getIdProvisioning",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}",
                defaults: new { controller = "IdentityProvisioning", action = "Get" });
            webApplication.MapControllerRoute("updateIdProvisioningDetails",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/details",
                defaults: new { controller = "IdentityProvisioning", action = "UpdateDetails" });
            webApplication.MapControllerRoute("updateIdProvisioningProperties",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/values",
                defaults: new { controller = "IdentityProvisioning", action = "UpdateProperties" });
            webApplication.MapControllerRoute("removeIdProvisioningMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "IdentityProvisioning", action = "RemoveMapper" });
            webApplication.MapControllerRoute("addIdProvisioningMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/mappers",
                defaults: new { controller = "IdentityProvisioning", action = "AddMapper" });
            webApplication.MapControllerRoute("extractRepresentations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{name}/{id}/enqueue",
                defaults: new { controller = "IdentityProvisioning", action = "Enqueue" });
            webApplication.MapControllerRoute("idProvisioningTestConnection",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/test",
                defaults: new { controller = "IdentityProvisioning", action = "TestConnection" });
            webApplication.MapControllerRoute("idProvisioningAllowedAttributes",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/{id}/allowedattributes",
                defaults: new { controller = "IdentityProvisioning", action = "GetAllowedAttributes" });
            webApplication.MapControllerRoute("searchIdProvisioningImport",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.IdentityProvisioning + "/import/.search",
                defaults: new { controller = "IdentityProvisioning", action = "SearchImport" });


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
            webApplication.MapControllerRoute("getIdProviderDefinitions",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthenticationSchemeProviders + "/defs",
                defaults: new { controller = "AuthenticationSchemeProviders", action = "GetDefinitions" });
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

            webApplication.MapControllerRoute("getAllAuthMethods",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods,
                defaults: new { controller = "AuthenticationMethods", action = "GetAll" });
            webApplication.MapControllerRoute("updateAuthMethodConfigurations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods + "/{amr}",
                defaults: new { controller = "AuthenticationMethods", action = "Update" });
            webApplication.MapControllerRoute("getAuthMethodConfigurations",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.AuthMethods + "/{amr}",
                defaults: new { controller = "AuthenticationMethods", action = "Get" });

            webApplication.MapControllerRoute("getAllRegistrationWorkflows",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows,
                defaults: new { controller = "RegistrationWorkflows", action = "GetAll" });
            webApplication.MapControllerRoute("getRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows + "/{id}",
                defaults: new { controller = "RegistrationWorkflows", action = "Get" });
            webApplication.MapControllerRoute("deleteRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows + "/{id}",
                defaults: new { controller = "RegistrationWorkflows", action = "Delete" });
            webApplication.MapControllerRoute("addRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows,
                defaults: new { controller = "RegistrationWorkflows", action = "Add" });
            webApplication.MapControllerRoute("updateRegistrationWorkflow",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.RegistrationWorkflows + "/{id}",
                defaults: new { controller = "RegistrationWorkflows", action = "Update" });

            webApplication.MapControllerRoute("addApiResource",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ApiResources,
                defaults: new { controller = "ApiResources", action = "Add" });
            webApplication.MapControllerRoute("searchApiResource",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.ApiResources + "/.search",
                defaults: new { controller = "ApiResources", action = "Search" });

            webApplication.MapControllerRoute("searchAuditing",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Auditing + "/.search",
                defaults: new { controller = "Auditing", action = "Search" });

            webApplication.MapControllerRoute("searchScopes",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/.search",
                defaults: new { controller = "Scopes", action = "Search" });
            webApplication.MapControllerRoute("getScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}",
                defaults: new { controller = "Scopes", action = "Get" });
            webApplication.MapControllerRoute("deleteScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}",
                defaults: new { controller = "Scopes", action = "Delete" });
            webApplication.MapControllerRoute("addScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes,
                defaults: new { controller = "Scopes", action = "Add" });
            webApplication.MapControllerRoute("updateScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}",
                defaults: new { controller = "Scopes", action = "Update" });
            webApplication.MapControllerRoute("addClaimMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/mappers",
                defaults: new { controller = "Scopes", action = "AddClaimMapper" });
            webApplication.MapControllerRoute("deleteClaimMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "Scopes", action = "RemoveClaimMapper" });
            webApplication.MapControllerRoute("updateClaimMapper",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{id}/mappers/{mapperId}",
                defaults: new { controller = "Scopes", action = "UpdateClaimMapper" });
            webApplication.MapControllerRoute("updateScopeResources",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Scopes + "/{name}/resources",
                defaults: new { controller = "Scopes", action = "UpdateResources" });

            webApplication.MapControllerRoute("searchCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/.search",
                defaults: new { controller = "CertificateAuthorities", action = "Search" });
            webApplication.MapControllerRoute("generateCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/generate",
                defaults: new { controller = "CertificateAuthorities", action = "Generate" });
            webApplication.MapControllerRoute("importCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/import",
                defaults: new { controller = "CertificateAuthorities", action = "Import" });
            webApplication.MapControllerRoute("addCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities,
                defaults: new { controller = "CertificateAuthorities", action = "Add" });
            webApplication.MapControllerRoute("removeCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}",
                defaults: new { controller = "CertificateAuthorities", action = "Remove" });
            webApplication.MapControllerRoute("getCertificateAuthorities",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}",
                defaults: new { controller = "CertificateAuthorities", action = "Get" });
            webApplication.MapControllerRoute("removeClientCertificate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}/clientcertificates/{clientCertificateId}",
                defaults: new { controller = "CertificateAuthorities", action = "RemoveClientCertificate" });
            webApplication.MapControllerRoute("addClientCertificate",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.CertificateAuthorities + "/{id}/clientcertificates",
                defaults: new { controller = "CertificateAuthorities", action = "AddClientCertificate" });

            webApplication.MapControllerRoute("searchClients",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/.search",
                defaults: new { controller = "Clients", action = "Search" });
            webApplication.MapControllerRoute("getAllClients",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients,
                defaults: new { controller = "Clients", action = "GetAll" });
            webApplication.MapControllerRoute("addClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients,
                defaults: new { controller = "Clients", action = "Add" });
            webApplication.MapControllerRoute("getClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}",
                defaults: new { controller = "Clients", action = "Get" });
            webApplication.MapControllerRoute("deleteClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}",
                defaults: new { controller = "Clients", action = "Delete" });
            webApplication.MapControllerRoute("updateClient",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}",
                defaults: new { controller = "Clients", action = "Update" });
            webApplication.MapControllerRoute("updateAdvancedClientSettings",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/advanced",
                defaults: new { controller = "Clients", action = "UpdateAdvanced" });
            webApplication.MapControllerRoute("removeClientScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/scopes/{name}",
                defaults: new { controller = "Clients", action = "RemoveScope" });
            webApplication.MapControllerRoute("addClientScope",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/scopes",
                defaults: new { controller = "Clients", action = "AddScope" });
            webApplication.MapControllerRoute("generateSigKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/sigkey/generate",
                defaults: new { controller = "Clients", action = "GenerateSigKey" });
            webApplication.MapControllerRoute("generateEncKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/enckey/generate",
                defaults: new { controller = "Clients", action = "GenerateEncKey" });
            webApplication.MapControllerRoute("addSigKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/sigkey",
                defaults: new { controller = "Clients", action = "AddSigKey" });
            webApplication.MapControllerRoute("addEncKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/enckey",
                defaults: new { controller = "Clients", action = "AddEncKey" });
            webApplication.MapControllerRoute("removeClientKey",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/keys/{keyId}",
                defaults: new { controller = "Clients", action = "RemoveKey" });
            webApplication.MapControllerRoute("updateClientCredentials",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/credentials",
                defaults: new { controller = "Clients", action = "UpdateCredentials" });
            webApplication.MapControllerRoute("addClientRole",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Clients + "/{id}/roles",
                defaults: new { controller = "Clients", action = "AddRole" });

            webApplication.MapControllerRoute("searchGroups",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/.search",
                defaults: new { controller = "Groups", action = "Search" });
            webApplication.MapControllerRoute("deleteGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/delete",
                defaults: new { controller = "Groups", action = "Delete" });
            webApplication.MapControllerRoute("getGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}",
                defaults: new { controller = "Groups", action = "Get" });
            webApplication.MapControllerRoute("addGroup",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups,
                defaults: new { controller = "Groups", action = "Add" });
            webApplication.MapControllerRoute("addGroupRole",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}/roles",
                defaults: new { controller = "Groups", action = "AddRole" });
            webApplication.MapControllerRoute("removeGroupRole",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Groups + "/{id}/roles/{roleId}",
                defaults: new { controller = "Groups", action = "RemoveRole" });

            webApplication.MapControllerRoute("getStats",
                pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Statistics,
                defaults: new { controller = "Statistics", action = "Get" });

            webApplication.MapControllerRoute("getAllRealms",
                pattern: Constants.EndPoints.Realms,
                defaults: new { controller = "Realms", action = "GetAll" });
            webApplication.MapControllerRoute("addRealm",
                pattern: Constants.EndPoints.Realms,
                defaults: new { controller = "Realms", action = "Add" });

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
