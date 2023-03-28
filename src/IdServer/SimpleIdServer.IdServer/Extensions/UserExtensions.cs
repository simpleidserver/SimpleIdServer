// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Domains
{
    public static class UserExtensions
    {
        public static bool HasOpenIDConsent(this User user, string prefix, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            return user.GetConsent(prefix, clientId, request, claims, authDetails, claimType) != null;
        }

        public static bool HasOpenIDConsent(this User user, string consentId)
        {
            return user.Consents.SingleOrDefault(c => c.Id == consentId) != null;
        }

        public static Consent GetConsent(this User user, string prefix, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
            => GetConsent(user, prefix, clientId, request.Scopes, claims, authDetails, claimType);

        public static Consent GetConsent(this User user, string prefix, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            var result = user.Consents.FirstOrDefault(c => c.Status == ConsentStatus.ACCEPTED && c.ClientId == clientId && c.Realm == prefix &&
                (scopes == null || (scopes.Where(s => s != Constants.StandardScopes.OpenIdScope.Name).All(s => c.Scopes.Any(sc => sc.Scope == s)))) &&
                (claims == null || (claims.Where(cl => cl.Type == claimType && cl.IsEssential && Constants.AllUserClaims.Contains(cl.Name)).All(cl => c.Claims.Any(scl => scl == cl.Name)))) &&
                ((authDetails == null || !authDetails.Any()) || (authDetails.All(d => c.AuthorizationDetails.Any(ad => ad.Type == d.Type && d.Actions.All(a => ad.Actions.Contains(a)) && d.Identifier == ad.Identifier))))
            );
            return result;
        }
    }
}
