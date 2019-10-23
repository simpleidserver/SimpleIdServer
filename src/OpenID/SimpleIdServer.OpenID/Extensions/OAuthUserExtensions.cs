// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class OAuthUserExtensions
    {
        private static Dictionary<string, string> CLAIM_MAPPINGS = new Dictionary<string, string>
        {
            { Jwt.Constants.UserClaims.Subject, ClaimTypes.NameIdentifier },
            { Jwt.Constants.UserClaims.Name, ClaimTypes.Name },
            { Jwt.Constants.UserClaims.Email, ClaimTypes.Email }
        };

        public static bool HasOpenIDConsent(this OAuthUser user, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claims, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            return user.GetConsent(clientId, scopes, claims, claimType) != null;
        }

        public static OAuthConsent GetConsent(this OAuthUser user, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claims, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            return user.Consents.FirstOrDefault(c => c.ClientId == clientId &&
                (scopes == null || (scopes.Where(s => s != SIDOpenIdConstants.StandardScopes.OpenIdScope.Name).All(s => c.Scopes.Any(sc => sc.Name == s)))) &&
                (claims == null || (claims.Where(cl => cl.Type == claimType && cl.IsEssential && Jwt.Constants.USER_CLAIMS.Contains(cl.Name))
                    .All(cl => c.Claims.Any(scl => scl == cl.Name)))));
        }

        public static List<Claim> ToClaims(this OAuthUser oauthUser)
        {
            var claims = new List<Claim>();
            foreach (var cl in oauthUser.Claims)
            {
                if (!CLAIM_MAPPINGS.ContainsKey(cl.Key))
                {
                    continue;
                }

                claims.Add(new Claim(CLAIM_MAPPINGS[cl.Key], cl.Value));
            }

            return claims;
        }
    }
}