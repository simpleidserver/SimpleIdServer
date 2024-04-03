// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IUserHelper
    {
        bool HasOpenIDConsent(User user, string prefix, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken);
        bool HasOpenIDConsent(User user, string consentId);
        Consent GetConsent(User user, string prefix, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken);
        Consent GetConsent(User user, string prefix, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken);
        void UpdatePicture(User user, IFormFile file, string issuer);
    }

    public class UserHelper : IUserHelper
    {
        public bool HasOpenIDConsent(User user, string prefix, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            return GetConsent(user, prefix, clientId, request, claims, authDetails, claimType) != null;
        }

        public bool HasOpenIDConsent(User user, string consentId)
        {
            return user.Consents.SingleOrDefault(c => c.Id == consentId) != null;
        }

        public Consent GetConsent(User user, string prefix, string clientId, GrantRequest request, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
            => GetConsent(user, prefix, clientId, request.Scopes, claims, authDetails, claimType);

        public virtual Consent GetConsent(User user, string prefix, string clientId, IEnumerable<string> scopes, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authDetails, AuthorizationClaimTypes claimType = AuthorizationClaimTypes.IdToken)
        {
            return user.Consents.FirstOrDefault(c => c.Status == ConsentStatus.ACCEPTED && c.ClientId == clientId && c.Realm == prefix &&
                (scopes == null || (scopes.Where(s => s != Constants.StandardScopes.OpenIdScope.Name).All(s => c.Scopes.Any(sc => sc.Scope == s)))) &&
                (claims == null || (claims.Where(cl => cl.Type == claimType && cl.IsEssential && Constants.AllUserClaims.Contains(cl.Name)).All(cl => c.Claims.Any(scl => scl == cl.Name)))) &&
                ((authDetails == null || !authDetails.Any()) || (authDetails.All(d =>
                {
                    return c.AuthorizationDetails.Any(ad => ad.Type == d.Type && d.Actions != null && d.Actions.All(a => ad.Actions.Contains(a)) && d.Identifier == ad.Identifier);
                })))
            );
        }

        public void UpdatePicture(User user, IFormFile file, string issuer)
        {
            var pictureBase64 = string.Empty;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var payload = ms.ToArray();
                pictureBase64 = Convert.ToBase64String(payload);
            }

            var pictureUrl = $"{issuer}/{Constants.EndPoints.Users}/{user.Id}/picture";
            user.EncodedPicture = pictureBase64;
            user.UpdateClaim(Constants.UserClaims.Picture, pictureUrl);
        }
    }
}
