﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientSecretJwtAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IJwtParser _jwtParser;

        public OAuthClientSecretJwtAuthenticationHandler(IJwtParser jwtParser)
        {
            _jwtParser = jwtParser;
        }

        public string AuthMethod => "client_secret_jwt";

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, BaseClient client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null)
            {
                throw new ArgumentNullException(nameof(authenticateInstruction));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(client.ClientSecret))
            {
                throw new OAuthException(errorCode, ErrorMessages.NO_CLIENT_SECRET);
            }

            var clientAssertion = authenticateInstruction.ClientAssertion;
            var isJweToken = _jwtParser.IsJweToken(clientAssertion);
            if (!isJweToken)
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
            }

            var clientId = authenticateInstruction.ClientIdFromHttpRequestBody;
            var jws = await _jwtParser.Decrypt(clientAssertion, clientId, client.ClientSecret, cancellationToken);
            if (string.IsNullOrWhiteSpace(jws))
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_DECRYPTION);
            }

            var isJwsToken = _jwtParser.IsJwsToken(jws);
            if (!isJwsToken)
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
            }

            JwsPayload payload = await _jwtParser.Unsign(clientAssertion, clientId, cancellationToken, errorCode);
            if (payload == null)
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_SIGNATURE);
            }

            return ValidateJwsPayLoad(payload, expectedIssuer, errorCode);
        }

        private bool ValidateJwsPayLoad(JwsPayload jwsPayload, string expectedIssuer, string errorCode)
        {
            var jwsIssuer = jwsPayload.GetIssuer();
            var jwsSubject = jwsPayload.GetClaimValue(SimpleIdServer.Jwt.Constants.UserClaims.Subject);
            var jwsAudiences = jwsPayload.GetAudiences();
            var expirationDateTime = jwsPayload.GetExpirationTime().ConvertFromUnixTimestamp();
            // 1. Check the client is correct.
            if (jwsSubject != jwsIssuer)
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_ISSUER);
            }

            // 2. Check if the audience is correct
            if (jwsAudiences == null || !jwsAudiences.Any() || !jwsAudiences.Any(j => j.Contains(expectedIssuer)))
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_AUDIENCES);
            }

            // 3. Check the expiration time
            if (DateTime.UtcNow > expirationDateTime)
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_EXPIRED);
            }

            return true;
        }
    }
}
