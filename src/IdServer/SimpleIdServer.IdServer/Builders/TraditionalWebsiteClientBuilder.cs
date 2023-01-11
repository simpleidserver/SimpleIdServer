// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class TraditionalWebsiteClientBuilder
    {
        private readonly Client _client;

        internal TraditionalWebsiteClientBuilder(Client client) { _client = client; }

        /// <summary>
        /// Add scope.
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(new Scope { Name = scope });
            return this;
        }

        /// <summary>
        /// Add signing key used to check the 'request' parameter.
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="alg"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder AddSigningKey(SigningCredentials signingCredentials, string alg)
        {
            var jsonWebKey = signingCredentials.SerializePublicJWK();
            jsonWebKey.Alg = alg;
            _client.Add(signingCredentials.Kid, jsonWebKey);
            return this;
        }

        public TraditionalWebsiteClientBuilder AddSigningKey(RsaSecurityKey securityKey, string alg = SecurityAlgorithms.RsaSha256) => AddSigningKey(new SigningCredentials(securityKey, alg), alg);

        /// <summary>
        /// Set the algorithm used to sign the request object.
        /// </summary>
        /// <param name="alg"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder SetRequestObjectSigning(string alg)
        {
            _client.RequestObjectSigningAlg = alg;
            return this;
        }

        /// <summary>
        /// Configure the algorithm to encrypt the request object.
        /// </summary>
        /// <param name="alg"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder SetRequestObjectEncryption(string alg = SecurityAlgorithms.RsaPKCS1, string enc = SecurityAlgorithms.Aes128CbcHmacSha256)
        {
            _client.RequestObjectEncryptionAlg = alg;
            _client.RequestObjectEncryptionEnc = enc;
            return this;
        }

        /// <summary>
        /// PKCE is an extension to the Authorization Code flow to prevent CSRF and 
        /// For more information: https://oauth.net/2/pkce/
        /// </summary>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder EnableClientPkceAuthentication()
        {
            _client.TokenEndPointAuthMethod = OAuthPKCEAuthenticationHandler.AUTH_METHOD;
            return this;
        }

        /// <summary>
        /// Allows client to continue to have a valid access token without further interaction with the user.
        /// </summary>
        /// <param name="refreshTokenExpirationTimeInSeconds"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder EnableRefreshTokenGrantType(double? refreshTokenExpirationTimeInSeconds = null)
        {
            _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            _client.RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds;
            return this;
        }

        /// <summary>
        /// Response type can return 'id_token'.
        /// </summary>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder EnableIdTokenInResponseType()
        {
            if (!_client.ResponseTypes.Contains(IdTokenResponseTypeHandler.RESPONSE_TYPE))
                _client.ResponseTypes.Add(IdTokenResponseTypeHandler.RESPONSE_TYPE);
            return this;
        }

        public Client Build() => _client;
    }
}
