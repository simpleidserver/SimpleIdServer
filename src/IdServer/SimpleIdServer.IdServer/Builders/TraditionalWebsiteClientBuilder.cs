// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;

namespace SimpleIdServer.IdServer.Builders
{
    public class TraditionalWebsiteClientBuilder
    {
        private readonly Client _client;

        internal TraditionalWebsiteClientBuilder(Client client) { _client = client; }

        public TraditionalWebsiteClientBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        /// <summary>
        /// Allows client to continue to have a valid access token without further interaction with the user.
        /// </summary>
        /// <param name="refreshTokenExpirationTimeInSeconds"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder AddRefreshTokenGrantType(double? refreshTokenExpirationTimeInSeconds = null)
        {
            _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            _client.RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds;
            return this;
        }

        /// <summary>
        /// PKCE is an extension to the Authorization Code flow to prevent CSRF and 
        /// For more information: https://oauth.net/2/pkce/
        /// </summary>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder UseClientPkceAuthentication()
        {
            _client.TokenEndPointAuthMethod = OAuthPKCEAuthenticationHandler.AUTH_METHOD;
            return this;
        }

        public Client Build() => _client;
    }
}
