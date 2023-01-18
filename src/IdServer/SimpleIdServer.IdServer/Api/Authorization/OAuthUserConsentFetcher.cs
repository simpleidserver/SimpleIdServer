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
        Consent FetchFromAuthorizationRequest(User oauthUser, JsonObject queryParameters);
        Consent BuildFromAuthorizationRequest(JsonObject queryParameters);
    }

    public class OAuthUserConsentFetcher : IUserConsentFetcher
    {
        public virtual Consent BuildFromAuthorizationRequest(JsonObject queryParameters)
        {
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return new Consent
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Scopes = scopes.ToList(),
                Claims = claims.Select(c => c.Name)
            };
        }

        public virtual Consent FetchFromAuthorizationRequest(User oauthUser, JsonObject queryParameters)
        {
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return oauthUser.GetConsent(clientId, scopes, claims, AuthorizationClaimTypes.IdToken);
        }
    }
}
