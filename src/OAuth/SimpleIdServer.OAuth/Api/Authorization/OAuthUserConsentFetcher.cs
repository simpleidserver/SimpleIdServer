// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public class OAuthUserConsentFetcher : IUserConsentFetcher
    {
        public virtual Consent BuildFromAuthorizationRequest(JsonObject queryParameters)
        {
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            return new Consent
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Scopes = scopes.Select(s => new Scope
                {
                    Name = s
                })
            };
        }

        public virtual Consent FetchFromAuthorizationRequest(User oauthUser, JsonObject queryParameters)
        {
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            return oauthUser.Consents.FirstOrDefault(c => queryParameters.GetClientIdFromAuthorizationRequest() == c.ClientId &&
                (scopes == null || (scopes.All(s => c.Scopes.Any(sc => sc.Name == s)))));
        }
    }
}
