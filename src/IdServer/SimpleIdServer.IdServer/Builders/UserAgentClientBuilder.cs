// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;

namespace SimpleIdServer.IdServer.Builders
{
    public class UserAgentClientBuilder
    {
        private readonly Client _client;

        internal UserAgentClientBuilder(Client client) { _client = client; }

        public UserAgentClientBuilder AddScope(params Scope[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        #region Response types

        /// <summary>
        /// Use implicit flow.
        /// Return id_token and token.
        /// </summary>
        /// <returns></returns>
        public UserAgentClientBuilder UseImplicitFlow()
        {
            _client.ResponseTypes = new List<string> { IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE };
            return this;
        }

        #endregion

        #region Translations

        /// <summary>
        /// Set client name.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public UserAgentClientBuilder SetClientName(string clientName, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            _client.Translations.Add(new Translation
            {
                Key = "client_name",
                Value = clientName,
                Language = language
            });
            return this;
        }

        #endregion

        #region Encryption and signature keys

        public UserAgentClientBuilder DisableIdTokenSignature()
        {
            _client.IdTokenSignedResponseAlg = SecurityAlgorithms.None;
            return this;
        }

        public UserAgentClientBuilder SetIdTokenSignatureAlg(string alg)
        {
            _client.IdTokenSignedResponseAlg = alg;
            return this;
        }

        public UserAgentClientBuilder SetIdTokenEncryption(string alg, string enc)
        {
            _client.IdTokenEncryptedResponseAlg= alg;
            _client.IdTokenEncryptedResponseEnc = enc;
            return this;
        }

        public UserAgentClientBuilder AddEncryptedKey(EncryptingCredentials credentials, SecurityKeyTypes keyType)
        {
            var jsonWebKey = credentials.SerializePublicJWK();
            jsonWebKey.Alg = credentials.Alg;
            _client.Add(credentials.Key.KeyId, jsonWebKey, Constants.JWKUsages.Enc, keyType);
            return this;
        }

        public UserAgentClientBuilder AddRSAEncryptedKey(RsaSecurityKey rsa, string alg, string enc, SecurityKeyTypes keyType) => AddEncryptedKey(new EncryptingCredentials(rsa, alg, enc), SecurityKeyTypes.RSA);

        #endregion

        public Client Build() => _client;
    }
}
