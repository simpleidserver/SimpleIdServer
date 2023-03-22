// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Authenticate.AssertionParsers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Authenticate.Handlers
{
    /// <summary>
    /// https://oauth.net/private-key-jwt/
    /// </summary>
    public class OAuthClientPrivateKeyJwtAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IClientHelper _clientHelper;
        private readonly IEnumerable<IClientAssertionParser> _assertionParsers;

        public OAuthClientPrivateKeyJwtAuthenticationHandler(IClientHelper clientHelper, IEnumerable<IClientAssertionParser> assertionParsers)
        {
            _clientHelper = clientHelper;
            _assertionParsers = assertionParsers;
        }

        public string AuthMethod => AUTH_METHOD;
        public const string AUTH_METHOD = "private_key_jwt";

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
            if (client == null) throw new ArgumentNullException(nameof(client));
            ValidateAuthenticateInstruction(authenticateInstruction, errorCode);

            var parser = _assertionParsers.First(p => p.Type == ClientJwtAssertionParser.TYPE);
            var clientAssertion = authenticateInstruction.ClientAssertion;
            var parseResult = parser.Parse(clientAssertion);
            if (parseResult.Status == ClientAssertionStatus.ERROR) throw new OAuthException(errorCode, parseResult.ErrorMessage);
            var token = parseResult.JsonWebToken;
            if (!token.IsSigned) throw new OAuthException(errorCode, ErrorMessages.CLIENT_ASSERTION_IS_NOT_SIGNED);

            var jsonWebKey = await _clientHelper.ResolveJsonWebKey(client, token.Kid, cancellationToken);
            if (jsonWebKey == null) throw new OAuthException(errorCode, ErrorMessages.CLIENT_ASSERTION_NOT_SIGNED_BY_KNOWN_JWK);
            var validationResult = new JsonWebTokenHandler().ValidateToken(clientAssertion, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = jsonWebKey
            });
            if (!validationResult.IsValid) throw new OAuthException(errorCode, validationResult.Exception.ToString());
            return ValidateJwsPayLoad(token, expectedIssuer, errorCode);
        }

        private void ValidateAuthenticateInstruction(AuthenticateInstruction authenticateInstruction, string errorCode)
        {
            if (authenticateInstruction.ClientAssertionType != ClientJwtAssertionParser.TYPE) throw new OAuthException(errorCode, string.Format(ErrorMessages.CLIENT_ASSERTION_TYPE_IS_UNEXPECTED, ClientJwtAssertionParser.TYPE));
            if (string.IsNullOrWhiteSpace(authenticateInstruction.ClientAssertion)) throw new OAuthException(errorCode, ErrorMessages.CLIENT_ASSERTION_IS_MISSING);
        }

        private bool ValidateJwsPayLoad(JsonWebToken jsonWebToken, string expectedIssuer, string errorCode)
        {
            var jwsIssuer = jsonWebToken.Issuer;
            var jwsSubject = jsonWebToken.Subject;
            var jwsAudiences = jsonWebToken.Audiences;
            var expirationDateTime = jsonWebToken.ValidTo;
            // 1. Check the client is correct.
            if (jwsSubject != jwsIssuer) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_ISSUER);
            // 2. Check if the audience is correct
            if (jwsAudiences == null || !jwsAudiences.Any() || !jwsAudiences.Any(j => j.Contains(expectedIssuer))) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_AUDIENCES);
            // 3. Check the expiration time
            if (DateTime.UtcNow > expirationDateTime) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_EXPIRED);
            return true;
        }
    }
}
