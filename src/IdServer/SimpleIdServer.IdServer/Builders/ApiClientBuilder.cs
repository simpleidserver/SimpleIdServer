// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Builders
{
    public class ApiClientBuilder
    {
        private readonly Client _client;

        internal ApiClientBuilder(Client client)
        {
            _client = client;
        }

        #region Signing Key

        /// <summary>
        /// Add signing key used to check the 'request' parameter.
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="alg"></param>
        /// <returns></returns>
        public ApiClientBuilder AddSigningKey(SigningCredentials signingCredentials, string alg, SecurityKeyTypes securityKey)
        {
            var jsonWebKey = signingCredentials.SerializePublicJWK();
            jsonWebKey.Alg = alg;
            _client.Add(signingCredentials.Kid, jsonWebKey, Constants.JWKUsages.Sig, securityKey);
            return this;
        }

        public ApiClientBuilder AddSigningKey(RsaSecurityKey securityKey, string alg = SecurityAlgorithms.RsaSha256) => AddSigningKey(new SigningCredentials(securityKey, alg), alg, SecurityKeyTypes.RSA);

        public ApiClientBuilder GenerateRSASigningKey(string keyid, string alg = SecurityAlgorithms.RsaSha256)
        {
            var sigKey = ClientKeyGenerator.GenerateRSASignatureKey(keyid, alg);
            return AddSigningKey(sigKey, alg, SecurityKeyTypes.RSA);
        }

        #endregion

        #region Grant-types

        public ApiClientBuilder EnableExchangeTokenGrantType(TokenExchangeTypes exchangeType)
        {
            _client.GrantTypes.Add(TokenExchangeHandler.GRANT_TYPE);
            _client.TokenExchangeType = exchangeType;
            _client.IsTokenExchangeEnabled = true;
            return this;
        }

        /// <summary>
        /// Allows the client to use UMA grant-type.
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder EnableUMAGrantType()
        {
            _client.GrantTypes.Add(UmaTicketHandler.GRANT_TYPE);
            return this;
        }

        /// <summary>
        /// Add password grant-type.
        /// Exchange user's credentials for an access token.
        /// </summary>
        /// <returns></returns>

        public ApiClientBuilder EnablePasswordGrantType()
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
            return EnablePasswordGrantType();
        }

        /// <summary>
        /// Allows client to continue to have a valid access token without further interaction with the user.
        /// </summary>
        /// <param name="refreshTokenExpirationTimeInSeconds"></param>
        /// <returns></returns>
        public ApiClientBuilder EnableRefreshTokenGrantType(double? refreshTokenExpirationTimeInSeconds = null)
        {
            _client.GrantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            _client.RefreshTokenExpirationTimeInSeconds = refreshTokenExpirationTimeInSeconds;
            return this;
        }

        #endregion

        #region Translations

        /// <summary>
        /// Set client name.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public ApiClientBuilder SetClientName(string clientName, string language = null)
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

        #endregion

        #region Client Authentication methods

        /// <summary>
        /// Use 'private_key_jwt' as authentication method.
        /// For more information : https://oauth.net/private-key-jwt/
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder UsePrivateKeyJwtAuthentication()
        {
            _client.TokenEndPointAuthMethod = OAuthClientPrivateKeyJwtAuthenticationHandler.AUTH_METHOD;
            return this;
        }

        /// <summary>
        /// Use 'client_secret_jwt' as authentication method.
        /// For more information : https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication
        /// </summary>
        /// <param name="jsonWebKeys"></param>
        /// <returns></returns>
        public ApiClientBuilder UseClientSecretJwtAuthentication()
        {
            _client.TokenEndPointAuthMethod = OAuthClientSecretJwtAuthenticationHandler.AUTH_METHOD;
            return this;
        }

        /// <summary>
        /// Use 'self_signed_tls_client_auth' as authentication method.
        /// For more information : https://www.rfc-editor.org/rfc/rfc8705.html#name-self-signed-method-metadata
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder UseClientSelfSignedAuthentication()
        {
            _client.TokenEndPointAuthMethod = OAuthClientSelfSignedTlsClientAuthenticationHandler.AUTH_METHOD;
            return this;
        }

        /// <summary>
        /// Use 'client_secret_basic' as authentication method.
        /// For more information : https://datatracker.ietf.org/doc/html/rfc7617.
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder UseClientSecretBasicAuthentication()
        {
            _client.TokenEndPointAuthMethod = OAuthClientSecretBasicAuthenticationHandler.AUTH_METHOD;
            return this;
        }

        /// <summary>
        /// Use 'tls_client_auth' as authentication method.
        /// For more information : https://oauth.net/2/mtls/
        /// </summary>
        /// <param name="subjectDn">Expected subject distinguished name of the certificate.</param>
        /// <param name="sanDns">Expected dNSName SAN entry in the certificate.</param>
        /// <param name="sanEmail">Expected rfc822Name SAN entry in the certificate.</param>
        /// <param name="sanIp">A string representation of an IP address in either dotted decimal notation (IPV4) or colon-delimited hexadecimal (IPV6) that is expected to be present as an iPAddress SAN entry in the certificate</param>
        /// <returns></returns>
        public ApiClientBuilder UseClientTlsAuthentication(string subjectDn, string sanDns = null, string sanEmail = null, string sanIp = null)
        {
            _client.TokenEndPointAuthMethod = OAuthClientTlsClientAuthenticationHandler.AUTH_METHOD;
            _client.TlsClientAuthSubjectDN = subjectDn;
            _client.TlsClientAuthSanDNS = sanDns;
            _client.TlsClientAuthSanEmail = sanEmail;
            _client.TlsClientAuthSanIP = sanIp;
            return this;
        }

        #endregion

        #region Others

        /// <summary>
        /// Set the access token type (jwt or reference).
        /// Default value is jwt.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ApiClientBuilder SetAccessTokenType(AccessTokenTypes type)
        {
            _client.AccessTokenType = type;
            return this;
        }

        /// <summary>
        /// Set the token expiration time in seconds.
        /// </summary>
        /// <param name="tokenExpirationTimeInSeconds"></param>
        /// <returns></returns>
        public ApiClientBuilder SetTokenExpirationTimeInSeconds(double tokenExpirationTimeInSeconds)
        {
            _client.TokenExpirationTimeInSeconds = tokenExpirationTimeInSeconds;
            return this;
        }

        /// <summary>
        /// Add a self signed certificate into the Json Web Key (JWK).
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder AddSelfSignedCertificate(string keyId, string subjectName = "cn=selfSigned")
        {
            var ecdsa = ECDsa.Create();
            var req = new CertificateRequest(subjectName, ecdsa, HashAlgorithmName.SHA256);
            var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(2));
            var key = new X509SecurityKey(cert)
            {
                KeyId = keyId
            };
            var jwk = JsonWebKeyConverter.ConvertFromX509SecurityKey(key);
            _client.SerializedJsonWebKeys.Add(new ClientJsonWebKey
            {
                Kid = keyId,
                Usage = Constants.JWKUsages.Sig,
                Alg = SecurityAlgorithms.RsaSha256,
                KeyType = SecurityKeyTypes.CERTIFICATE,
                SerializedJsonWebKey = JsonWebKeySerializer.Write(jwk)
            });
            return this;
        }

        #endregion

        #region Scopes

        public ApiClientBuilder AddScope(params Scope[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        /// <summary>
        /// Client will act as a UMA resource server.
        /// Grant access to the scope uma_protection.
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder ActAsUMAResourceServer()
        {
            AddScope(Constants.DefaultScopes.UmaProtection);
            return this;
        }

        #endregion

        #region DPOP

        /// <summary>
        /// DPOP Proof is required
        /// </summary>
        /// <returns></returns>
        public ApiClientBuilder UseDPOPProof(bool isNonceRequired = false)
        {
            _client.DPOPBoundAccessTokens = true;
            _client.IsDPOPNonceRequired = isNonceRequired;
            return this;
        }

        #endregion

        public Client Build() => _client;
    }
}
