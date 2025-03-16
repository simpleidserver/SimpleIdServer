// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Linq;
using System.Threading;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Builders
{
    public class MobileClientBuilder
    {
        private readonly Client _client;

        internal MobileClientBuilder(Client client) { _client = client; }

        public Client Client => _client;

        #region Grant types

        /// <summary>
        /// Allows client to continue to have a valid access token without further interaction with the user.
        /// </summary>
        /// <param name="refreshTokenExpirationTimeInSeconds"></param>
        /// <returns></returns>
        public MobileClientBuilder EnableRefreshTokenGrantType(double? refreshTokenExpirationTimeInSeconds = null)
        {
            _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            _client.RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds;
            return this;
        }

        #endregion

        #region Response type

        /// <summary>
        /// Response type can return 'id_token'.
        /// </summary>
        /// <returns></returns>
        public MobileClientBuilder EnableIdTokenInResponseType()
        {
            if (!_client.ResponseTypes.Contains(IdTokenResponseTypeHandler.RESPONSE_TYPE))
                _client.ResponseTypes.Add(IdTokenResponseTypeHandler.RESPONSE_TYPE);
            return this;
        }

        /// <summary>
        /// Response type can return 'token'.
        /// </summary>
        /// <returns></returns>
        public MobileClientBuilder EnableTokenInResponseType()
        {
            if (!_client.ResponseTypes.Contains(TokenResponseTypeHandler.RESPONSE_TYPE))
                _client.ResponseTypes.Add(TokenResponseTypeHandler.RESPONSE_TYPE);
            return this;
        }

        #endregion

        #region Scopes

        /// <summary>
        /// Add scope.
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public MobileClientBuilder AddScope(params Scope[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        /// <summary>
        /// Enable the access to the grants token.
        /// </summary>
        /// <returns></returns>
        public MobileClientBuilder EnableAccessToGrantsApi()
        {
            if (!_client.Scopes.Any(s => s.Name == Config.DefaultScopes.GrantManagementQuery.Name))
                _client.Scopes.Add(Config.DefaultScopes.GrantManagementQuery);

            if (!_client.Scopes.Any(s => s.Name == Config.DefaultScopes.GrantManagementRevoke.Name))
                _client.Scopes.Add(Config.DefaultScopes.GrantManagementRevoke);

            return this;
        }

        /// <summary>
        /// Enable offline_access.
        /// This scope value requests that an OAUTH2.0 refresh token be issued that can be used to obtain an access token that grants access to the End-User's UserInfo Endpoint even when the End-User is not present (not logged-in).
        /// </summary>
        /// <returns></returns>
        public MobileClientBuilder EnableOfflineAccess()
        {
            AddScope(Config.DefaultScopes.OfflineAccessScope);
            if (!_client.GrantTypes.Contains(RefreshTokenHandler.GRANT_TYPE))
                _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            return this;
        }

        #endregion

        #region Signing Key

        /// <summary>
        /// Add signing key used to check the 'request' parameter.
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="alg"></param>
        /// <returns></returns>
        public MobileClientBuilder AddSigningKey(SigningCredentials signingCredentials, string alg, SecurityKeyTypes keyType)
        {
            var jsonWebKey = signingCredentials.SerializePublicJWK();
            jsonWebKey.Alg = alg;
            _client.Add(signingCredentials.Kid, jsonWebKey, DefaultTokenSecurityAlgs.JwkUsages.Sig, keyType);
            return this;
        }

        public MobileClientBuilder AddSigningKey(RsaSecurityKey securityKey, string alg = SecurityAlgorithms.RsaSha256) => AddSigningKey(new SigningCredentials(securityKey, alg), alg, SecurityKeyTypes.RSA);

        #endregion

        #region Encryption Key

        public MobileClientBuilder AddEncryptedKey(EncryptingCredentials credentials, SecurityKeyTypes keyType)
        {
            var jsonWebKey = credentials.SerializePublicJWK();
            jsonWebKey.Alg = credentials.Alg;
            _client.Add(credentials.Key.KeyId, jsonWebKey, DefaultTokenSecurityAlgs.JwkUsages.Enc, keyType);
            return this;
        }

        public MobileClientBuilder AddRSAEncryptedKey(RsaSecurityKey rsa, string alg, string enc) => AddEncryptedKey(new EncryptingCredentials(rsa, alg, enc), SecurityKeyTypes.RSA);

        #endregion

        #region Request object

        /// <summary>
        /// Set the algorithm used to sign the request object.
        /// </summary>
        /// <param name="alg"></param>
        /// <returns></returns>
        public MobileClientBuilder SetRequestObjectSigning(string alg)
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
        public MobileClientBuilder SetRequestObjectEncryption(string alg = SecurityAlgorithms.RsaPKCS1, string enc = SecurityAlgorithms.Aes128CbcHmacSha256)
        {
            _client.RequestObjectEncryptionAlg = alg;
            _client.RequestObjectEncryptionEnc = enc;
            return this;
        }

        #endregion

        #region Subject type

        /// <summary>
        /// Set the subject_type.
        /// </summary>
        /// <param name="subjectType"></param>
        /// <returns></returns>
        public MobileClientBuilder SetSubjectType(string subjectType)
        {
            _client.SubjectType = subjectType;
            return this;
        }

        /// <summary>
        /// Use pairwise subject_type.
        /// </summary>
        /// <param name="salt">Salt used to generate the pairwise subject.</param>
        /// <returns></returns>
        public MobileClientBuilder SetPairwiseSubjectType(string salt)
        {
            _client.SubjectType = PairWiseSubjectTypeBuidler.SUBJECT_TYPE;
            _client.PairWiseIdentifierSalt = salt;
            return this;
        }

        #endregion

        #region User info

        /// <summary>
        /// Set the algorithm to sign the userinfo response.
        /// </summary>
        /// <param name="signingAlg"></param>
        /// <returns></returns>
        public MobileClientBuilder SetUserInfoSignedResponseAlg(string alg = SecurityAlgorithms.RsaSha256)
        {
            _client.UserInfoSignedResponseAlg = alg;
            return this;
        }

        /// <summary>
        /// Set the algorithm to encrypt the userinfo response.
        /// </summary>
        /// <param name="alg"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public MobileClientBuilder SetUserInfoEncryption(string alg = SecurityAlgorithms.RsaPKCS1, string enc = SecurityAlgorithms.Aes128CbcHmacSha256)
        {
            _client.UserInfoEncryptedResponseAlg = alg;
            _client.UserInfoEncryptedResponseEnc = enc;
            return this;
        }

        #endregion

        #region Client authentication

        /// <summary>
        /// Use 'tls_client_auth' as authentication method.
        /// For more information : https://oauth.net/2/mtls/
        /// </summary>
        /// <param name="subjectDn">Expected subject distinguished name of the certificate.</param>
        /// <param name="sanDns">Expected dNSName SAN entry in the certificate.</param>
        /// <param name="sanEmail">Expected rfc822Name SAN entry in the certificate.</param>
        /// <param name="sanIp">A string representation of an IP address in either dotted decimal notation (IPV4) or colon-delimited hexadecimal (IPV6) that is expected to be present as an iPAddress SAN entry in the certificate</param>
        /// <returns></returns>
        public MobileClientBuilder UseClientTlsAuthentication(string subjectDn, string sanDns = null, string sanEmail = null, string sanIp = null)
        {
            _client.TokenEndPointAuthMethod = OAuthClientTlsClientAuthenticationHandler.AUTH_METHOD;
            _client.TlsClientAuthSubjectDN = subjectDn;
            _client.TlsClientAuthSanDNS = sanDns;
            _client.TlsClientAuthSanEmail = sanEmail;
            _client.TlsClientAuthSanIP = sanIp;
            return this;
        }

        #endregion

        #region Other parameters

        /// <summary>
        /// Set the sector_identifier_uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public MobileClientBuilder SetSectorIdentifierUri(string uri)
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
        public MobileClientBuilder SetDefaultMaxAge(int defaultMaxAge)
        {
            _client.DefaultMaxAge = defaultMaxAge;
            return this;
        }

        /// <summary>
        /// resource parameter must be required
        /// </summary>
        /// <returns></returns>
        public MobileClientBuilder ResourceParameterIsRequired()
        {
            _client.IsResourceParameterRequired = true;
            return this;
        }

        /// <summary>
        /// Set client name.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public MobileClientBuilder SetClientName(string clientName, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Domains.Language.Default;

            _client.Translations.Add(new Translation
            {
                Key = "client_name",
                Value = clientName,
                Language = language
            });
            return this;
        }

        /// <summary>
        /// Set the logo uri.
        /// </summary>
        /// <param name="logoUri"></param>
        /// <returns></returns>
        public MobileClientBuilder SetClientLogoUri(string logoUri, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Domains.Language.Default;

            _client.Translations.Add(new Translation
            {
                Key = "logo_uri",
                Value = logoUri,
                Language = language
            });
            return this;
        }

        #endregion

        public Client Build() => _client;
    }
}
