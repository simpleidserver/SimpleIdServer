// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class OAuthUserExtensions
    {
        public static bool HasOpenIDConsent(this OAuthUser user, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claims, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            return user.GetConsent(clientId, scopes, claims, claimType) != null;
        }

        public static bool HasOpenIDConsent(this OAuthUser user, string consentId)
        {
            return user.Consents.SingleOrDefault(c => c.Id == consentId) != null;
        }

        public static OAuthConsent GetConsent(this OAuthUser user, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizationRequestClaimParameter> claims, AuthorizationRequestClaimTypes claimType = AuthorizationRequestClaimTypes.IdToken)
        {
            return user.Consents.FirstOrDefault(c => c.ClientId == clientId &&
                (scopes == null || (scopes.Where(s => s != SIDOpenIdConstants.StandardScopes.OpenIdScope.Name).All(s => c.Scopes.Any(sc => sc.Name == s)))) &&
                (claims == null || (claims.Where(cl => cl.Type == claimType && cl.IsEssential && Jwt.Constants.USER_CLAIMS.Contains(cl.Name))
                    .All(cl => c.Claims.Any(scl => scl == cl.Name)))));
        }
    }
}