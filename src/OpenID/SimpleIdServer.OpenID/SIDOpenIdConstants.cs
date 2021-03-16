// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID
{
    public static class SIDOpenIdConstants
    {
        public static List<string[]> HybridWorkflows = new List<string[]>
        {
            new string[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE },
            new string[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, Api.Authorization.ResponseTypes.IdTokenResponseTypeHandler.RESPONSE_TYPE },
            new string[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, Api.Authorization.ResponseTypes.IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE }
        };
        public static class EndPoints
        {
            public const string OpenIDConfiguration = ".well-known/openid-configuration";
            public const string UserInfo = "userinfo";
            public const string CheckSession = "check_session";
            public const string EndSession = "end_session";
            public const string EndSessionCallback = "end_session_callback";
            public const string BCAuthorize = OAuth.Constants.EndPoints.MtlsPrefix + "/bc-authorize";
            public const string BCDeviceRegistration = "bc-device-registration";
        }

        public static class StandardClaims
        {
            public static OpenIdScopeClaim Name = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Name, true);
            public static OpenIdScopeClaim FamilyName = new OpenIdScopeClaim(Jwt.Constants.UserClaims.FamilyName, true);
            public static OpenIdScopeClaim UniqueName = new OpenIdScopeClaim(Jwt.Constants.UserClaims.UniqueName, true);
            public static OpenIdScopeClaim GivenName = new OpenIdScopeClaim(Jwt.Constants.UserClaims.GivenName, true);
            public static OpenIdScopeClaim MiddleName = new OpenIdScopeClaim(Jwt.Constants.UserClaims.MiddleName, true);
            public static OpenIdScopeClaim NickName = new OpenIdScopeClaim(Jwt.Constants.UserClaims.NickName, true);
            public static OpenIdScopeClaim PreferredUserName = new OpenIdScopeClaim(Jwt.Constants.UserClaims.PreferredUserName, true);
            public static OpenIdScopeClaim Profile = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Profile, true);
            public static OpenIdScopeClaim Picture = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Picture, true);
            public static OpenIdScopeClaim WebSite = new OpenIdScopeClaim(Jwt.Constants.UserClaims.WebSite, true);
            public static OpenIdScopeClaim Gender = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Gender, true);
            public static OpenIdScopeClaim BirthDate = new OpenIdScopeClaim(Jwt.Constants.UserClaims.BirthDate, true);
            public static OpenIdScopeClaim ZoneInfo = new OpenIdScopeClaim(Jwt.Constants.UserClaims.ZoneInfo, true);
            public static OpenIdScopeClaim Locale = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Locale, true);
            public static OpenIdScopeClaim UpdatedAt = new OpenIdScopeClaim(Jwt.Constants.UserClaims.UpdatedAt, true);
            public static OpenIdScopeClaim Email = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Email, true);
            public static OpenIdScopeClaim EmailVerified = new OpenIdScopeClaim(Jwt.Constants.UserClaims.EmailVerified, true);
            public static OpenIdScopeClaim Address = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Address, true);
            public static OpenIdScopeClaim PhoneNumber = new OpenIdScopeClaim(Jwt.Constants.UserClaims.PhoneNumber, true);
            public static OpenIdScopeClaim PhoneNumberVerified = new OpenIdScopeClaim(Jwt.Constants.UserClaims.PhoneNumberVerified, true);
            public static OpenIdScopeClaim Role = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Role, true);
            public static OpenIdScopeClaim Subject = new OpenIdScopeClaim(Jwt.Constants.UserClaims.Subject, true);
        }

        public static class StandardScopes
        {
            public static OpenIdScope Profile = new OpenIdScope
            {
                Name = "profile",
                Claims = new List<OpenIdScopeClaim>
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
                IsExposedInConfigurationEdp = true
            };
            public static OpenIdScope Email = new OpenIdScope
            {
                Name = "email",
                Claims = new List<OpenIdScopeClaim>
                {
                    StandardClaims.Email,
                    StandardClaims.EmailVerified
                },
                IsExposedInConfigurationEdp = true
            };
            public static OpenIdScope Address = new OpenIdScope
            {
                Name = "address",
                Claims = new List<OpenIdScopeClaim>
                {
                    StandardClaims.Address
                },
                IsExposedInConfigurationEdp = true
            };
            public static OpenIdScope Phone = new OpenIdScope
            {
                Name = "phone",
                Claims = new List<OpenIdScopeClaim>
                {
                    StandardClaims.PhoneNumber,
                    StandardClaims.PhoneNumberVerified
                },
                IsExposedInConfigurationEdp = true
            };
            public static OpenIdScope Role = new OpenIdScope
            {
                Name = "role",
                Claims = new List<OpenIdScopeClaim>
                {
                    StandardClaims.Role
                },
                IsExposedInConfigurationEdp = true
            };
            public static OpenIdScope OpenIdScope = new OpenIdScope
            {
                Name = "openid",
                Claims = new List<OpenIdScopeClaim>
                {
                    StandardClaims.Subject
                },
                IsExposedInConfigurationEdp = true
            };
            public static OpenIdScope OfflineAccessScope = new OpenIdScope
            {
                Name = "offline_access",
                IsExposedInConfigurationEdp = true
            };
        }
    }
}