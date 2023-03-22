// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Authenticate.Handlers
{
    /// <summary>
    /// https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication
    /// </summary>
    public class OAuthClientSecretJwtAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IClientHelper _clientHelper;
        private readonly ILogger<OAuthClientSecretJwtAuthenticationHandler> _logger;
        public string AuthMethod => AUTH_METHOD;
        public const string AUTH_METHOD = "client_secret_jwt";

        public OAuthClientSecretJwtAuthenticationHandler(IClientHelper clientHelper, ILogger<OAuthClientSecretJwtAuthenticationHandler> logger)
        {
            _clientHelper = clientHelper;
            _logger = logger;
        }

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
            if (client == null) throw new ArgumentNullException(nameof(client));
            var clientAssertion = authenticateInstruction.ClientAssertion;
            var handler = new JsonWebTokenHandler();
            var token = handler.ReadJsonWebToken(clientAssertion);
            if (!token.IsEncrypted) throw new OAuthException(errorCode, ErrorMessages.CLIENT_ASSERTION_IS_NOT_ENCRYPTED);
            var encryptionSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(client.ClientSecret));
            string jws = null;
            try
            {
                jws = handler.DecryptToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    TokenDecryptionKey = encryptionSecurityKey
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new OAuthException(errorCode, ErrorMessages.CLIENT_ASSERTION_CANNOT_BE_DECRYPTED);
            }

            token = handler.ReadJsonWebToken(jws);
            var jsonWebKey = await _clientHelper.ResolveJsonWebKey(client, token.Kid, cancellationToken);
            if (jsonWebKey == null) throw new OAuthException(errorCode, ErrorMessages.CLIENT_ASSERTION_NOT_SIGNED_BY_KNOWN_JWK);
            var validationResult = handler.ValidateToken(jws, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = jsonWebKey
            });
            if (!validationResult.IsValid) throw new OAuthException(errorCode, validationResult.Exception.ToString());
            ValidateJwsPayLoad(token, expectedIssuer, errorCode);
            return true;
        }

        private void ValidateJwsPayLoad(JsonWebToken jsonWebToken, string expectedIssuer, string errorCode)
        {
            var jwsIssuer = jsonWebToken.Issuer;
            var jwsSubject = jsonWebToken.Subject;
            var jwsAudiences = jsonWebToken.Audiences;
            var expirationDateTime = jsonWebToken.ValidTo;
            // 1. Check the client is correct.
            if (jwsSubject != jwsIssuer) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_ISSUER);
            // 2. Check if the audience is correct
            if (jwsAudiences == null || !jwsAudiences.Any() || !jwsAudiences.Any(j => j.Contains(expectedIssuer)))
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_AUDIENCES);
            // 3. Check the expiration time
            if (DateTime.UtcNow > expirationDateTime)
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_EXPIRED);
        }
    }
}
