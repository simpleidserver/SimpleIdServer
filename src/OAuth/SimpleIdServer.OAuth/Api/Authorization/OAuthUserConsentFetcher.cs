// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using System;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public class OAuthUserConsentFetcher : IUserConsentFetcher
    {
        public virtual OAuthConsent BuildFromAuthorizationRequest(JObject queryParameters)
        {
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            return new OAuthConsent
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Scopes = scopes.Select(s => new OAuthScope
                {
                    Name = s
                })
            };
        }

        public virtual OAuthConsent FetchFromAuthorizationRequest(OAuthUser oauthUser, JObject queryParameters)
        {
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            return oauthUser.Consents.FirstOrDefault(c => queryParameters.GetClientIdFromAuthorizationRequest() == c.ClientId &&
                (scopes == null || (scopes.All(s => c.Scopes.Any(sc => sc.Name == s)))));
        }
    }
}
