// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Domains
{
    public static class UserExtensions
    {
        public static byte[] GetOTPKey(this User user) => user.OTPKey.ConvertToBase32();

        public static bool HasOpenIDConsent(this User user, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            return user.GetConsent(clientId, request, claims, claimType) != null;
        }

        public static bool HasOpenIDConsent(this User user, string consentId)
        {
            return user.Consents.SingleOrDefault(c => c.Id == consentId) != null;
        }

        public static Consent GetConsent(this User user, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
            => GetConsent(user, clientId, request.Scopes, claims, claimType);

        public static Consent GetConsent(this User user, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizedClaim> claims, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            return user.Consents.FirstOrDefault(c => c.ClientId == clientId &&
                (scopes == null || (scopes.Where(s => s != Constants.StandardScopes.OpenIdScope.Name).All(s => c.Scopes.Contains(s)))) &&
                (claims == null || (claims.Where(cl => cl.Type == claimType && cl.IsEssential && Constants.AllUserClaims.Contains(cl.Name)).All(cl => c.Claims.Any(scl => scl == cl.Name))))
            );
        }
    }
}
