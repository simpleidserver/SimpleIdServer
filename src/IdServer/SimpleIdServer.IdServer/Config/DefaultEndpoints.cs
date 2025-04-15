// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Config;

public static class DefaultEndpoints
{
    public const string Token = "token";
    public const string TokenRevoke = "token/revoke";
    public const string TokenInfo = "token_info";
    public const string Jwks = "jwks";
    public const string Authorization = "authorization";
    public const string AuthorizationCallback = "authorization/callback";
    public const string Registration = "register";
    public const string OAuthConfiguration = ".well-known/oauth-authorization-server";
    public const string OpenIDConfiguration = ".well-known/openid-configuration";
    public const string IdServerConfiguration = ".well-known/idserver-configuration";
    public const string FidoConfiguration = ".well-known/fido-configuration";
    public const string UMAConfiguration = ".well-known/uma2-configuration";
    public const string Form = "form";
    public const string AuthSchemeProviders = "authschemeproviders";
    public const string ClientManagement = "management/clients";
    public const string BCAuthorize = "bc-authorize";
    public const string BCCallback = "bc-callback";
    public const string MtlsPrefix = "mtls";
    public const string MtlsToken = MtlsPrefix + "/" + Token;
    public const string MtlsBCAuthorize = MtlsPrefix + "/" + BCAuthorize;
    public const string UserInfo = "userinfo";
    public const string CheckSession = "check_session";
    public const string EndSession = "end_session";
    public const string EndSessionCallback = "end_session_callback";
    public const string ActiveSession = "active_session";
    public const string Grants = "grants";
    public const string UMAPermissions = "perm";
    public const string UMAResources = "rreguri";
    public const string IdentityProvisioning = "provisioning";
    public const string PushedAuthorizationRequest = "par";
    public const string Users = "users";
    public const string Networks = "networks";
    public const string DeviceAuthorization = "device_authorization";
    public const string AuthenticationClassReferences = "acrs";
    public const string AuthenticationSchemeProviders = "idproviders";
    public const string FIDORegistration = "fido/u2f/registration";
    public const string FIDOAuthentication = "fido/u2f/authentication";
    public const string AuthMethods = "authmethods";
    public const string RegistrationWorkflows = "registrationworkflows";
    public const string ApiResources = "apiresources";
    public const string Scopes = "scopes";
    public const string Auditing = "auditing";
    public const string CertificateAuthorities = "cas";
    public const string Clients = "clients";
    public const string Statistics = "stats";
    public const string Realms = "realms";
    public const string Groups = "groups";
    public const string Languages = "languages";
    public const string ErrorMessages = "errormessages";
    public const string Workflows = "workflows";
    public const string Forms = "forms";
    public const string RecurringJobs = "recurringjobs";
    public const string ConfigurationDefs = "confdefs";
    public const string Templates = "templates";
}
