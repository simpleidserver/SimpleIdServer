// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Authorization
{
    public interface IUserConsentFetcher
    {
        Consent FetchFromAuthorizationRequest(string realm, User oauthUser, JsonObject queryParameters);
        Consent BuildFromAuthorizationRequest(string realm, JsonObject queryParameters);
    }

    public class OAuthUserConsentFetcher : IUserConsentFetcher
    {
        public virtual Consent BuildFromAuthorizationRequest(string realm, JsonObject queryParameters)
        {
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return new Consent
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Scopes = scopes.ToList(),
                Claims = claims.Select(c => c.Name),
                CreateDateTime = DateTime.UtcNow,
                Realm = realm
            };
        }

        public virtual Consent FetchFromAuthorizationRequest(string realm, User oauthUser, JsonObject queryParameters)
        {
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            var authDetails = queryParameters.GetAuthorizationDetailsFromAuthorizationRequest();
            return oauthUser.GetConsent(realm, clientId, scopes, claims, authDetails, AuthorizationClaimTypes.IdToken);
        }
    }
}
