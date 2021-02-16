// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Jwt
{
    public class Constants
    {
        public static class OAuthClaims
        {
            public static string Issuer = "iss";
            public static string Audiences = "aud";
            public static string ExpirationTime = "exp";
            public static string Iat = "iat";
            public static string AuthenticationTime = "auth_time";
            public static string Nonce = "nonce";
            public static string Acr = "acr";
            public static string Amr = "amr";
            public static string Azp = "azp";
            /// <summary>
            /// Unique identifier of the JWT.
            /// </summary>
            public static string Jti = "jti";
            /// <summary>
            /// Access token hash value
            /// </summary>
            public static string AtHash = "at_hash";
            /// <summary>
            /// Authorization code hash value
            /// </summary>
            public static string CHash = "c_hash";
            public static string ClientId = "client_id";
            public static string Scopes = "scope";
            public static string Claims = "claims";
            public static string Cnf = "cnf";
            public static string X5TS256 = "x5t#S256";
        }

        public static class UserClaims
        {
            public static string Subject = "sub";
            public static string Name = "name";
            public static string GivenName = "given_name";
            public static string FamilyName = "family_name";
            public static string MiddleName = "middle_name";
            public static string NickName = "nickname";
            public static string PreferredUserName = "preferred_username";
            public static string Profile = "profile";
            public static string Picture = "picture";
            public static string WebSite = "website";
            public static string Email = "email";
            public static string EmailVerified = "email_verified";
            public static string Gender = "gender";
            public static string BirthDate = "birthdate";
            public static string ZoneInfo = "zoneinfo";
            public static string Locale = "locale";
            public static string PhoneNumber = "phone_number";
            public static string PhoneNumberVerified = "phone_number_verified";
            public static string Address = "address";
            public static string UpdatedAt = "updated_at";
            public static string Role = "role";
            public static string ScimId = "scim_id";
            public static string ScimLocation = "scim_location";
            public static string UniqueName = "unique_name";
        }

        public static ICollection<string> USER_CLAIMS = new List<string>
        {
            UserClaims.Subject, UserClaims.Name, UserClaims.GivenName, UserClaims.FamilyName, UserClaims.MiddleName,
            UserClaims.NickName, UserClaims.PreferredUserName, UserClaims.Profile, UserClaims.Picture, UserClaims.WebSite,
            UserClaims.Email, UserClaims.EmailVerified, UserClaims.Gender, UserClaims.BirthDate, UserClaims.ZoneInfo,
            UserClaims.Locale, UserClaims.PhoneNumber, UserClaims.PhoneNumberVerified, UserClaims.Address, UserClaims.UpdatedAt,
            UserClaims.Role, UserClaims.ScimId, UserClaims.ScimLocation
        };
    }
}