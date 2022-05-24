// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using System;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class JwsPayloadExtensions
    {
        public static string GetIssuer(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetClaimValue(OAuthClaims.Issuer);
        }

        public static string[] GetAudiences(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetArrayClaim(OAuthClaims.Audiences);
        }

        public static double GetExpirationTime(this JwsPayload jwsPayload) 
        {
            return jwsPayload.GetDoubleClaim(OAuthClaims.ExpirationTime);
        }

        public static double GetIat(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetDoubleClaim(OAuthClaims.Iat);
        }

        public static string[] GetScopes(this JwsPayload jwsPayload)
        {
            var arr = jwsPayload.GetArrayClaim(OAuthClaims.Scopes);
            if (arr != null && arr.Length == 1) return arr[0].Split(' ');
            return arr;
        }

        public static string GetAzp(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetClaimValue(OAuthClaims.Azp);
        }

        public static double GetNbf(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetDoubleClaim(OAuthClaims.Nbf);
        }

        public static string GetJti(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetClaimValue(OAuthClaims.Jti);
        }

        public static DateTime? GetAuthTime(this JwsPayload jwsPayload)
        {
            var authDateTime = jwsPayload.GetDoubleClaim(OAuthClaims.AuthenticationTime);
            if (authDateTime.Equals(default(double)))
            {
                return null;
            }

            return authDateTime.ConvertFromUnixTimestamp();
        }
    }
}