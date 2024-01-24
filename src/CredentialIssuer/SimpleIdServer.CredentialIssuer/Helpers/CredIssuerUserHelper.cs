// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.CredentialIssuer.Helpers
{
    public class CredIssuerUserHelper : UserHelper
    {
        public override Consent GetConsent(User user, string prefix, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            return user.Consents.FirstOrDefault(c => c.Status == ConsentStatus.ACCEPTED && c.ClientId == clientId && c.Realm == prefix &&
                (scopes == null || (scopes.Where(s => s != SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope.Name).All(s => c.Scopes.Any(sc => sc.Scope == s)))) &&
                (claims == null || (claims.Where(cl => cl.Type == claimType && cl.IsEssential && SimpleIdServer.IdServer.Constants.AllUserClaims.Contains(cl.Name)).All(cl => c.Claims.Any(scl => scl == cl.Name)))) &&
                ((authDetails == null || !authDetails.Any()) || (authDetails.All(d =>
                {
                    if (d.Type == SimpleIdServer.IdServer.CredentialIssuer.Constants.StandardAuthorizationDetails.OpenIdCredential) return c.AuthorizationDetails.Any(ad => ad.Type == d.Type && d.GetTypes() != null && d.GetTypes().All(t => ad.GetTypes().Contains(t)));
                    return c.AuthorizationDetails.Any(ad => ad.Type == d.Type && d.Actions != null && d.Actions.All(a => ad.Actions.Contains(a)) && d.Identifier == ad.Identifier);
                })))
            );
        }
    }
}
