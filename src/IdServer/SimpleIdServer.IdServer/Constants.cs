// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer;

public static class Constants
{
    public const string SubjectAlternativeNameOid = "2.5.29.17";
    public const string AuthenticatedPolicyName = "authenticated";
    public const string AreaPwd = "pwd";
    public const string LogoutUserKey = "otherUser";
    public const string RealmKey = "realm";
    public const string DefaultExternalCookieAuthenticationScheme = "ExternalCookies";
    public const string DefaultCertificateAuthenticationScheme = "Certificate";
    public const string AuthorizationHeaderName = "Authorization";
    public const string DPOPHeaderName = "DPoP";
    public const string DPOPNonceHeaderName = "DPoP-Nonce";
    public const string AlgDir = "dir";
    public const string ParFormatKey = "urn:ietf:params:oauth:request_uri";
    public const string IdProviderSeparator = ";";
    public const string LDAPDistinguishedName = "LDAP_DISTINGUISHEDNAME";
    public const string DefaultNotificationMode = "console";
    public const string DefaultRealm = "master";
    public const string DefaultCurrentAcrCookieName = "currentAmr";
    public const string DefaultRememberMeCookieName = "RememberMe";
    public const string Prefix = "prefix";
    public const string DefaultLanguage = "en";
    public const string ConsoleAmr = "console";
}
