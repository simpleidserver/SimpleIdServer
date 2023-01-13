// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.SubjectTypeBuilders;

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
        /// Set the subject_type.
        /// </summary>
        /// <param name="subjectType"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder SetSubjectType(string subjectType)
        {
            _client.SubjectType = subjectType;
            return this;
        }

        /// <summary>
        /// Use pairwise subject_type.
        /// </summary>
        /// <param name="salt">Salt used to generate the pairwise subject.</param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder SetPairwiseSubjectType(string salt)
        {
            _client.SubjectType = PairWiseSubjectTypeBuidler.SUBJECT_TYPE;
            _client.PairWiseIdentifierSalt = salt;
            return this;
        }

        /// <summary>
        /// Set the sector_identifier_uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder SetSectorIdentifierUri(string uri)
        {
            _client.SectorIdentifierUri = uri;
            return this;
        }

        /// <summary>
        /// Set the default Maximum Authentication Age.
        /// Specifies that the End-User MUST be actively authenticated if the End-User was authenticated longer ago than the specified number of seconds.
        /// </summary>
        /// <param name="defaultMaxAge"></param>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder SetDefaultMaxAge(int defaultMaxAge)
        {
            _client.DefaultMaxAge = defaultMaxAge;
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

        /// <summary>
        /// Enable offline_access.
        /// This scope value requests that an OAUTH2.0 refresh token be issued that can be used to obtain an access token that grants access to the End-User's UserInfo Endpoint even when the End-User is not present (not logged-in).
        /// </summary>
        /// <returns></returns>
        public TraditionalWebsiteClientBuilder EnableOfflineAccess()
        {
            AddScope(Constants.StandardScopes.OfflineAccessScope.Name);
            if (!_client.GrantTypes.Contains(RefreshTokenHandler.GRANT_TYPE))
                _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            return this;
        }

        public Client Build() => _client;
    }
}
