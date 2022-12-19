// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SimpleIdServer.OAuth.Builders
{
    public class ApiClientBuilder
    {
        private readonly Client _client;

        internal ApiClientBuilder(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Add password grant-type.
        /// Exchange user's credentials for an access token.
        /// </summary>
        /// <returns></returns>

        public ApiClientBuilder AddPasswordGrantType()
        {
            _client.GrantTypes.Add(PasswordHandler.GRANT_TYPE);
            return this;
        }

        /// <summary>
        /// Use only the password grant-type.
        /// Exchange user's credentials for an access token.
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder UseOnlyPasswordGrantType()
        {
            _client.GrantTypes.Clear();
            return AddPasswordGrantType();
        }

        /// <summary>
        /// Allows client to continue to have a valid access token without further interaction with the user.
        /// </summary>
        /// <param name="refreshTokenExpirationTimeInSeconds"></param>
        /// <returns></returns>
        public ApiClientBuilder AddRefreshTokenGrantType(double? refreshTokenExpirationTimeInSeconds = null)
        {
            _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            _client.RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds;
            return this;
        }

        /// <summary>
        /// Use 'private_key_jwt' as authentication method.
        /// For more information : https://oauth.net/private-key-jwt/
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder UsePrivateKeyJwtAuthentication(params JsonWebKey[] jsonWebKeys)
        {
            _client.TokenEndPointAuthMethod = OAuthClientPrivateKeyJwtAuthenticationHandler.AUTH_METHOD;
            if (jsonWebKeys != null)
            {
                foreach (var jsonWebKey in jsonWebKeys.Select(j => new ClientJsonWebKey
                {
                    Kid = j.Kid,
                    SerializedJsonWebKey = JsonExtensions.SerializeToJson(j)
                }))
                    _client.SerializedJsonWebKeys.Add(jsonWebKey);
            }

            return this;
        }

        /// <summary>
        /// Use 'client_secret_jwt' as authentication method.
        /// For more information : https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication
        /// </summary>
        /// <param name="jsonWebKeys"></param>
        /// <returns></returns>
        public ApiClientBuilder UseClientSecretJwtAuthentication(params JsonWebKey[] jsonWebKeys)
        {
            // TODO : Check the password !!!
            _client.TokenEndPointAuthMethod = OAuthClientSecretJwtAuthenticationHandler.AUTH_METHOD;
            if (jsonWebKeys != null)
            {
                foreach(var jsonWebKey in jsonWebKeys.Select(j => new ClientJsonWebKey
                {
                    Kid = j.Kid,
                    SerializedJsonWebKey = JsonExtensions.SerializeToJson(j)
                }))
                    _client.SerializedJsonWebKeys.Add(jsonWebKey);
            }

            return this;
        }

        public ApiClientBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        public Client Build() => _client;
    }
}
