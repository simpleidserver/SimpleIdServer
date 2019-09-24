// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using System;
using System.Linq;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDUserConsentFetcher : IUserConsentFetcher
    {
        public OAuthConsent FetchFromAuthorizationRequest(OAuthUser oauthUser, JObject queryParameters)
        {
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return oauthUser.GetConsent(clientId, scopes, claims, AuthorizationRequestClaimTypes.IdToken);
        }

        public virtual OAuthConsent BuildFromAuthorizationRequest(JObject queryParameters)
        {
            var scopes = queryParameters.GetScopesFromAuthorizationRequest();
            var clientId = queryParameters.GetClientIdFromAuthorizationRequest();
            var claims = queryParameters.GetClaimsFromAuthorizationRequest();
            return new OAuthConsent
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                Scopes = scopes.Select(s => new OAuthScope
                {
                    Name = s
                }),
                Claims = claims.Select(c => new OAuthClaim(c.Name))
            };
        }
    }
}
