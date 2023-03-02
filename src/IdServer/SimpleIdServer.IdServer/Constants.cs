// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.IdServer
{
    public static class Constants
    {
        public static class JWKUsages
        {
            public const string Enc = "enc";
            public const string Sig = "sig";
        }

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
            public const string Grants = "grants";
            public const string UMAPermissions = "perm";
            public const string UMAResources = "rreguri";
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
            public const string Authenticated = "authenticated";
        }

        public static ICollection<string> AllSigningAlgs = new List<string>
        {
            // SecurityAlgorithms.HmacSha256,
            // SecurityAlgorithms.HmacSha384,
            // SecurityAlgorithms.HmacSha512,
            // SecurityAlgorithms.RsaSsaPssSha256,
            // SecurityAlgorithms.RsaSsaPssSha384,
            // SecurityAlgorithms.RsaSsaPssSha512,
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.RsaSha384,
            SecurityAlgorithms.RsaSha512,
            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.EcdsaSha384,
            SecurityAlgorithms.EcdsaSha512,
            SecurityAlgorithms.None
        };

        public static ICollection<string> AllEncAlgs = new List<string>
        {
            SecurityAlgorithms.RsaPKCS1,
            SecurityAlgorithms.RsaOAEP,
            // SecurityAlgorithms.Aes128KW,
            // SecurityAlgorithms.Aes192KW,
            // SecurityAlgorithms.Aes256KW,
            // SecurityAlgorithms.EcdhEs,
            // SecurityAlgorithms.EcdhEsA128kw,
            // SecurityAlgorithms.EcdhEsA192kw,
            // SecurityAlgorithms.EcdhEsA256kw
        };

        public static ICollection<string> AllEncryptions = new List<string>
        {
            SecurityAlgorithms.Aes128CbcHmacSha256,
            SecurityAlgorithms.Aes192CbcHmacSha384,
            SecurityAlgorithms.Aes256CbcHmacSha512,
            // SecurityAlgorithms.Aes128Gcm,
            // SecurityAlgorithms.Aes192Gcm,
            // SecurityAlgorithms.Aes256Gcm
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
            public static ScopeClaimMapper Subject = ScopeClaimMapper.CreateOpenIdSubjectClaim();
            public static ScopeClaimMapper Name = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.Name, JwtRegisteredClaimNames.Name, nameof(User.Firstname));
            public static ScopeClaimMapper FamilyName = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.FamilyName, JwtRegisteredClaimNames.FamilyName, nameof(User.Lastname));
            public static ScopeClaimMapper UpdatedAt = ScopeClaimMapper.CreateOpenIdPropertyClaim(UserClaims.UpdatedAt, UserClaims.UpdatedAt, nameof(User.UpdateDateTime), TokenClaimJsonTypes.DATETIME);
            public static ScopeClaimMapper Email = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.Email, JwtRegisteredClaimNames.Email, nameof(User.Email));
            public static ScopeClaimMapper EmailVerified = ScopeClaimMapper.CreateOpenIdPropertyClaim(UserClaims.EmailVerified, UserClaims.EmailVerified, nameof(User.EmailVerified), TokenClaimJsonTypes.BOOLEAN);
            public static ScopeClaimMapper UniqueName = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.UniqueName, JwtRegisteredClaimNames.UniqueName, JwtRegisteredClaimNames.UniqueName);
            public static ScopeClaimMapper GivenName = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.GivenName); 
            public static ScopeClaimMapper MiddleName = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.MiddleName, UserClaims.MiddleName, UserClaims.MiddleName); 
            public static ScopeClaimMapper NickName = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.NickName, UserClaims.NickName, UserClaims.NickName); 
            public static ScopeClaimMapper PreferredUserName = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.PreferredUserName, UserClaims.PreferredUserName, UserClaims.PreferredUserName); 
            public static ScopeClaimMapper Profile = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.Profile, UserClaims.Profile, UserClaims.Profile); 
            public static ScopeClaimMapper Picture = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.Picture, UserClaims.Picture, UserClaims.Picture); 
            public static ScopeClaimMapper WebSite = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Website, JwtRegisteredClaimNames.Website, JwtRegisteredClaimNames.Website); 
            public static ScopeClaimMapper Gender = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Gender, JwtRegisteredClaimNames.Gender, JwtRegisteredClaimNames.Gender);
            public static ScopeClaimMapper BirthDate = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Birthdate, JwtRegisteredClaimNames.Birthdate, JwtRegisteredClaimNames.Birthdate);
            public static ScopeClaimMapper ZoneInfo = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.ZoneInfo, UserClaims.ZoneInfo, UserClaims.ZoneInfo); 
            public static ScopeClaimMapper Locale = ScopeClaimMapper.CreateOpenIdAttributeClaim(UserClaims.Locale, UserClaims.Locale, UserClaims.Locale);
            public static ScopeClaimMapper Address = ScopeClaimMapper.CreateOpenIdAddressClaim(UserClaims.Address);
            public static ScopeClaimMapper PhoneNumber = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumber);
            public static ScopeClaimMapper PhoneNumberVerified = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.PhoneNumberVerified, JwtRegisteredClaimNames.PhoneNumberVerified, JwtRegisteredClaimNames.PhoneNumberVerified, TokenClaimJsonTypes.BOOLEAN);
            public static ScopeClaimMapper Role = ScopeClaimMapper.CreateOpenIdAttributeClaimArray(UserClaims.Role, UserClaims.Role, UserClaims.Role);
            public static ScopeClaimMapper ScimId = ScopeClaimMapper.CreateOpenIdAttributeClaim("scim_id", "scim_id", "scim_id");
            public static ScopeClaimMapper SAMLNameIdentifier = ScopeClaimMapper.CreateSAMLNameIdentifierClaim();
            public static ScopeClaimMapper SAMLName = ScopeClaimMapper.CreateSAMLPropertyClaim("name", ClaimTypes.Name, nameof(User.Firstname));
        }

        public static class StandardScopes
        {
            public static Scope Profile = new Scope
            {
                Name = "profile",
                ClaimMappers = new List<ScopeClaimMapper>
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
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Email = new Scope
            {
                Name = "email",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Email,
                    StandardClaims.EmailVerified
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Address = new Scope
            {
                Name = "address",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Address
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Phone = new Scope
            {
                Name = "phone",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.PhoneNumber,
                    StandardClaims.PhoneNumberVerified
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope Role = new Scope
            {
                Name = "role",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Role
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope OpenIdScope = new Scope
            {
                Name = "openid",
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.Subject
                },
                Type = ScopeTypes.IDENTITY,
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope OfflineAccessScope = new Scope
            {
                Type = ScopeTypes.IDENTITY,
                Name = "offline_access",
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope ScimScope = new Scope
            {
                Type = ScopeTypes.IDENTITY,
                Name = "scim",
                IsExposedInConfigurationEdp = true,
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.ScimId
                },
                Protocol = ScopeProtocols.OPENID,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope GrantManagementQuery = new Scope
            {
                Type = ScopeTypes.IDENTITY,
                Name = "grant_management_query",
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = false,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope GrantManagementRevoke = new Scope
            {
                Type = ScopeTypes.IDENTITY,
                Name = "grant_management_revoke",
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = false,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope UmaProtection = new Scope
            {
                Type = ScopeTypes.IDENTITY,
                Name = "uma_protection",
                Protocol = ScopeProtocols.OPENID,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static Scope SAMLProfile = new Scope
            {
                Type = ScopeTypes.IDENTITY,
                Name = "saml_profile",
                Protocol = ScopeProtocols.SAML,
                IsExposedInConfigurationEdp = false,
                ClaimMappers = new List<ScopeClaimMapper>
                {
                    StandardClaims.SAMLNameIdentifier,
                    StandardClaims.SAMLName
                },
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
        public const string DefaultExternalCookieAuthenticationScheme = "ExternalCookies";
        public const string DefaultCertificateAuthenticationScheme = "Certificate";
        public static string AuthorizationHeaderName = "Authorization";
        public const string Prefix = "prefix";
        /// <summary>
        /// Direct use of a shared symmetric key as the CEK.
        /// </summary>
        public const string AlgDir = "dir";
    }
}