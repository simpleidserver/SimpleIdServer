// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer
{
    public static class Constants
    {
        public static class EndPoints
        {
            public const string Token = "token";
            public const string TokenRevoke = "token/revoke";
            public const string TokenInfo = "token_info";
            public const string Jwks = "jwks";
            public const string Authorization = "authorization";
            public const string Registration = "register";
            public const string OAuthConfiguration = ".well-known/oauth-authorization-server";
            public const string OpenIDConfiguration = ".well-known/openid-configuration";
            public const string Form = "form";
            public const string AuthSchemeProviders = "authschemeproviders";
            public const string ClientManagement = "management/clients";
            public const string BCAuthorize = "bc-authorize";
            public const string BCAuthorizeCallback = "bc-authorize/callback";
            public const string MtlsPrefix = "mtls";
            public const string MtlsToken = MtlsPrefix + "/" + Token;
            public const string MtlsBCAuthorize = MtlsPrefix + "/" + BCAuthorize;
            public const string UserInfo = "userinfo";
            public const string CheckSession = "check_session";
            public const string EndSession = "end_session";
            public const string EndSessionCallback = "end_session_callback";
            public const string Grants = "grants";
        }

        public static List<string> AllStandardNotificationModes = new List<string>
        {
            StandardNotificationModes.Ping,
            StandardNotificationModes.Poll,
            StandardNotificationModes.Push
        };

        public static List<string> AllStandardGrantManagementActions = new List<string>
        {
            StandardGrantManagementActions.Create,
            StandardGrantManagementActions.Merge,
            StandardGrantManagementActions.Replace
        };

        public static class StandardGrantManagementActions
        {
            public const string Create = "create";
            public const string Merge = "merge";
            public const string Replace = "replace";
        }

        public static class StandardNotificationModes
        {
            public const string Poll = "poll";
            public const string Ping = "ping";
            public const string Push = "push";
        }

        public static class ScopeNames
        {
            public const string Register = "register";
            public const string AuthSchemeProvider = "manage_authschemeprovider";
        }

        public static class CertificateOIDS
        {
            public const string SubjectAlternativeName = "2.5.29.17";
        }

        public class Policies
        {
            public const string Register = "register";
            public const string AuthSchemeProvider = "manage_authschemeprovider";
            public const string Authenticated = "authenticated";
        }

        public static ICollection<string> AllSigningAlgs = new List<string>
        {
            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.EcdsaSha384,
            SecurityAlgorithms.EcdsaSha512,
            SecurityAlgorithms.HmacSha256,
            SecurityAlgorithms.HmacSha384,
            SecurityAlgorithms.HmacSha512,
            SecurityAlgorithms.RsaSsaPssSha256,
            SecurityAlgorithms.RsaSsaPssSha384,
            SecurityAlgorithms.RsaSsaPssSha512,
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.RsaSha384,
            SecurityAlgorithms.RsaSha512,
            SecurityAlgorithms.None
        };

        public static ICollection<string> AllEncAlgs = new List<string>
        {
            SecurityAlgorithms.RsaPKCS1,
            SecurityAlgorithms.RsaOAEP,
            SecurityAlgorithms.Aes128KW,
            SecurityAlgorithms.Aes192KW,
            SecurityAlgorithms.Aes256KW,
            SecurityAlgorithms.EcdhEs,
            SecurityAlgorithms.EcdhEsA128kw,
            SecurityAlgorithms.EcdhEsA192kw,
            SecurityAlgorithms.EcdhEsA256kw
        };

        public static ICollection<string> AllEncryptions = new List<string>
        {
            SecurityAlgorithms.Aes128CbcHmacSha256,
            SecurityAlgorithms.Aes192CbcHmacSha384,
            SecurityAlgorithms.Aes256CbcHmacSha512,
            SecurityAlgorithms.Aes128Gcm,
            SecurityAlgorithms.Aes192Gcm,
            SecurityAlgorithms.Aes256Gcm
        };

        public static class UserClaims
        {
            public static string MiddleName = "middle_name";
            public static string NickName = "nickname";
            public static string PreferredUserName = "preferred_username";
            public static string Profile = "profile";
            public static string Picture = "picture";
            public static string BirthDate = "birthdate";
            public static string ZoneInfo = "zoneinfo";
            public static string Locale = "locale";
            public static string UpdatedAt = "updated_at";
            public static string EmailVerified = "email_verified";
            public static string Address = "address";
            public static string Role = "role";
            public static string ScimId = "scim_id";
            public static string ScimLocation = "scim_location";
            public static string Events = "events";
        }

        public static class StandardClaims
        {
            public static ScopeClaim Name = new ScopeClaim(JwtRegisteredClaimNames.Name, true);
            public static ScopeClaim FamilyName = new ScopeClaim(JwtRegisteredClaimNames.FamilyName, true);
            public static ScopeClaim UniqueName = new ScopeClaim(JwtRegisteredClaimNames.UniqueName, true);
            public static ScopeClaim GivenName = new ScopeClaim(JwtRegisteredClaimNames.GivenName, true);
            public static ScopeClaim MiddleName = new ScopeClaim(UserClaims.MiddleName, true);
            public static ScopeClaim NickName = new ScopeClaim(UserClaims.NickName, true);
            public static ScopeClaim PreferredUserName = new ScopeClaim(UserClaims.PreferredUserName, true);
            public static ScopeClaim Profile = new ScopeClaim(UserClaims.Profile, true);
            public static ScopeClaim Picture = new ScopeClaim(UserClaims.Picture, true);
            public static ScopeClaim WebSite = new ScopeClaim(JwtRegisteredClaimNames.Website, true);
            public static ScopeClaim Gender = new ScopeClaim(JwtRegisteredClaimNames.Gender, true);
            public static ScopeClaim BirthDate = new ScopeClaim(UserClaims.BirthDate, true);
            public static ScopeClaim ZoneInfo = new ScopeClaim(UserClaims.ZoneInfo, true);
            public static ScopeClaim Locale = new ScopeClaim(UserClaims.Locale, true);
            public static ScopeClaim UpdatedAt = new ScopeClaim(UserClaims.UpdatedAt, true);
            public static ScopeClaim Email = new ScopeClaim(JwtRegisteredClaimNames.Email, true);
            public static ScopeClaim EmailVerified = new ScopeClaim(UserClaims.EmailVerified, true);
            public static ScopeClaim Address = new ScopeClaim(UserClaims.Address, true);
            public static ScopeClaim PhoneNumber = new ScopeClaim(JwtRegisteredClaimNames.PhoneNumber, true);
            public static ScopeClaim PhoneNumberVerified = new ScopeClaim(JwtRegisteredClaimNames.PhoneNumberVerified, true);
            public static ScopeClaim Role = new ScopeClaim(UserClaims.Role, true);
            public static ScopeClaim Subject = new ScopeClaim(JwtRegisteredClaimNames.Sub, true);
        }

        public static class StandardScopes
        {
            public static Scope Profile = new Scope
            {
                Name = "profile",
                Claims = new List<ScopeClaim>
                {
                    StandardClaims.Name,
                    StandardClaims.FamilyName,
                    StandardClaims.UniqueName,
                    StandardClaims.GivenName,
                    StandardClaims.MiddleName,
                    StandardClaims.NickName,
                    StandardClaims.PreferredUserName,
                    StandardClaims.Profile,
                    StandardClaims.Picture,
                    StandardClaims.WebSite,
                    StandardClaims.Gender,
                    StandardClaims.BirthDate,
                    StandardClaims.ZoneInfo,
                    StandardClaims.Locale,
                    StandardClaims.UpdatedAt
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Email = new Scope
            {
                Name = "email",
                Claims = new List<ScopeClaim>
                {
                    StandardClaims.Email,
                    StandardClaims.EmailVerified
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Address = new Scope
            {
                Name = "address",
                Claims = new List<ScopeClaim>
                {
                    StandardClaims.Address
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Phone = new Scope
            {
                Name = "phone",
                Claims = new List<ScopeClaim>
                {
                    StandardClaims.PhoneNumber,
                    StandardClaims.PhoneNumberVerified
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Role = new Scope
            {
                Name = "role",
                Claims = new List<ScopeClaim>
                {
                    StandardClaims.Role
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope OpenIdScope = new Scope
            {
                Name = "openid",
                Claims = new List<ScopeClaim>
                {
                    StandardClaims.Subject
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope OfflineAccessScope = new Scope
            {
                Name = "offline_access",
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope ScimScope = new Scope
            {
                Name = "scim",
                IsExposedInConfigurationEdp = true,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim("scim_id", true)
                },
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope GrantManagementQuery = new Scope
            {
                Name = "grant_management_query",
                IsExposedInConfigurationEdp = false,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope GrantManagementRevoke = new Scope
            {
                Name = "grant_management_revoke",
                IsExposedInConfigurationEdp = false,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }

        public static class StandardAcrs
        {
            public static AuthenticationContextClassReference FirstLevelAssurance = new AuthenticationContextClassReference
            {
                AuthenticationMethodReferences = new[] { Areas.Password },
                Name = "sid-load-01",
                DisplayName = "First level of assurance"
            };
        }

        public static ICollection<string> AllUserClaims = new List<string>
        {
            JwtRegisteredClaimNames.Sub, JwtRegisteredClaimNames.Name, JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.FamilyName, UserClaims.MiddleName,
            UserClaims.NickName, UserClaims.PreferredUserName, UserClaims.Profile, UserClaims.Picture, JwtRegisteredClaimNames.Website,
            JwtRegisteredClaimNames.Email, UserClaims.EmailVerified, JwtRegisteredClaimNames.Gender, UserClaims.BirthDate, UserClaims.ZoneInfo,
            UserClaims.Locale, JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumberVerified, UserClaims.Address, UserClaims.UpdatedAt,
            UserClaims.Role, UserClaims.ScimId, UserClaims.ScimLocation
        };

        public static class Areas
        {
            public const string Password = "pwd";
        }

        public const string DefaultOIDCAuthenticationScheme = "OIDC";
        public const string DefaultCertificateAuthenticationScheme = "Certificate";
        public static string AuthorizationHeaderName = "Authorization";
        public const string Prefix = "prefix";
        /// <summary>
        /// Direct use of a shared symmetric key as the CEK.
        /// </summary>
        public const string AlgDir = "dir";
    }
}