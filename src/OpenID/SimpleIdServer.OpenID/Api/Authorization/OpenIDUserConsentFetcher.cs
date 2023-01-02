// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDUserConsentFetcher : IUserConsentFetcher
    {
        public Consent FetchFromAuthorizationRequest(User oauthUser, JsonObject queryParameters)
        {
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return oauthUser.GetConsent(clientId, scopes, claims, AuthorizationRequestClaimTypes.IdToken);
        }

        public virtual Consent BuildFromAuthorizationRequest(JsonObject queryParameters)
        {
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return new Consent
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Scopes = scopes.ToList(),
                Claims = claims.Select(c => c.Name)
            };
        }
    }
}
