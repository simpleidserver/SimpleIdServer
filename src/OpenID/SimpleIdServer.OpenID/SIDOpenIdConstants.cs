// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID
{
    public static class SIDOpenIdConstants
    {
        public const string ExternalAuthenticationScheme = "ExternalAuthentication";
        public static List<string> AllStandardNotificationModes = new List<string>
        {
            StandardNotificationModes.Ping,
            StandardNotificationModes.Poll,
            StandardNotificationModes.Push
        };

        public static class StandardNotificationModes
        {
            public const string Poll = "poll";
            public const string Ping = "ping";
            public const string Push = "push";
        }

        public static class EndPoints
        {
            public const string OpenIDConfiguration = ".well-known/openid-configuration";
            public const string UserInfo = "userinfo";
            public const string CheckSession = "check_session";
            public const string EndSession = "end_session";
            public const string EndSessionCallback = "end_session_callback";
            public const string MTLSBCAuthorize = OAuth.Constants.EndPoints.MtlsPrefix + "/" + BCAuthorize;
            public const string BCAuthorize = "bc-authorize";
            public const string BCDeviceRegistration = "bc-device-registration";
            public const string Metadata = "metadata";
        }

        public static class StandardClaims
        {
            public static OAuthScopeClaim Name = new OAuthScopeClaim(Jwt.Constants.UserClaims.Name, true);
            public static OAuthScopeClaim FamilyName = new OAuthScopeClaim(Jwt.Constants.UserClaims.FamilyName, true);
            public static OAuthScopeClaim UniqueName = new OAuthScopeClaim(Jwt.Constants.UserClaims.UniqueName, true);
            public static OAuthScopeClaim GivenName = new OAuthScopeClaim(Jwt.Constants.UserClaims.GivenName, true);
            public static OAuthScopeClaim MiddleName = new OAuthScopeClaim(Jwt.Constants.UserClaims.MiddleName, true);
            public static OAuthScopeClaim NickName = new OAuthScopeClaim(Jwt.Constants.UserClaims.NickName, true);
            public static OAuthScopeClaim PreferredUserName = new OAuthScopeClaim(Jwt.Constants.UserClaims.PreferredUserName, true);
            public static OAuthScopeClaim Profile = new OAuthScopeClaim(Jwt.Constants.UserClaims.Profile, true);
            public static OAuthScopeClaim Picture = new OAuthScopeClaim(Jwt.Constants.UserClaims.Picture, true);
            public static OAuthScopeClaim WebSite = new OAuthScopeClaim(Jwt.Constants.UserClaims.WebSite, true);
            public static OAuthScopeClaim Gender = new OAuthScopeClaim(Jwt.Constants.UserClaims.Gender, true);
            public static OAuthScopeClaim BirthDate = new OAuthScopeClaim(Jwt.Constants.UserClaims.BirthDate, true);
            public static OAuthScopeClaim ZoneInfo = new OAuthScopeClaim(Jwt.Constants.UserClaims.ZoneInfo, true);
            public static OAuthScopeClaim Locale = new OAuthScopeClaim(Jwt.Constants.UserClaims.Locale, true);
            public static OAuthScopeClaim UpdatedAt = new OAuthScopeClaim(Jwt.Constants.UserClaims.UpdatedAt, true);
            public static OAuthScopeClaim Email = new OAuthScopeClaim(Jwt.Constants.UserClaims.Email, true);
            public static OAuthScopeClaim EmailVerified = new OAuthScopeClaim(Jwt.Constants.UserClaims.EmailVerified, true);
            public static OAuthScopeClaim Address = new OAuthScopeClaim(Jwt.Constants.UserClaims.Address, true);
            public static OAuthScopeClaim PhoneNumber = new OAuthScopeClaim(Jwt.Constants.UserClaims.PhoneNumber, true);
            public static OAuthScopeClaim PhoneNumberVerified = new OAuthScopeClaim(Jwt.Constants.UserClaims.PhoneNumberVerified, true);
            public static OAuthScopeClaim Role = new OAuthScopeClaim(Jwt.Constants.UserClaims.Role, true);
            public static OAuthScopeClaim Subject = new OAuthScopeClaim(Jwt.Constants.UserClaims.Subject, true);
        }

        public static class StandardScopes
        {
            public static OAuthScope Profile = new OAuthScope
            {
                Name = "profile",
                Claims = new List<OAuthScopeClaim>
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
            public static OAuthScope Email = new OAuthScope
            {
                Name = "email",
                Claims = new List<OAuthScopeClaim>
                {
                    StandardClaims.Email,
                    StandardClaims.EmailVerified
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static OAuthScope Address = new OAuthScope
            {
                Name = "address",
                Claims = new List<OAuthScopeClaim>
                {
                    StandardClaims.Address
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static OAuthScope Phone = new OAuthScope
            {
                Name = "phone",
                Claims = new List<OAuthScopeClaim>
                {
                    StandardClaims.PhoneNumber,
                    StandardClaims.PhoneNumberVerified
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static OAuthScope Role = new OAuthScope
            {
                Name = "role",
                Claims = new List<OAuthScopeClaim>
                {
                    StandardClaims.Role
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static OAuthScope OpenIdScope = new OAuthScope
            {
                Name = "openid",
                Claims = new List<OAuthScopeClaim>
                {
                    StandardClaims.Subject
                },
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static OAuthScope OfflineAccessScope = new OAuthScope
            {
                Name = "offline_access",
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            public static OAuthScope ScimScope = new OAuthScope
            {
                Name = "scim",
                IsExposedInConfigurationEdp = true,
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim("scim_id", true)
                },
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }
    }
}