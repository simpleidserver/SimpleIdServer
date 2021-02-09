// Copyright (c) SimpleIdServer. All rights reserved.
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
    public class OAuthClientPrivateKeyJwtAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IJwsGenerator _jwsGenerator;
        private readonly IJwtParser _jwtParser;

        public OAuthClientPrivateKeyJwtAuthenticationHandler(IJwsGenerator jwsGenerator, IJwtParser jwtParser)
        {
            _jwsGenerator = jwsGenerator;
            _jwtParser = jwtParser;
        }

        public string AuthMethod => "private_key_jwt";

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, OAuthClient client, string expectedIssuer, CancellationToken cancellationToken)
        {
            if (authenticateInstruction == null)
            {
                throw new ArgumentNullException(nameof(authenticateInstruction));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var clientAssertion = authenticateInstruction.ClientAssertion;
            var isJwsToken = _jwtParser.IsJwsToken(clientAssertion);
            if (!isJwsToken)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
            }

            var jwsPayload = _jwsGenerator.ExtractPayload(clientAssertion);
            if (jwsPayload == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
            }

            var clientId = jwsPayload.GetIssuer();
            var payload = await _jwtParser.Unsign(clientAssertion, clientId, cancellationToken);
            if (payload == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.BAD_CLIENT_ASSERTION_SIGNATURE);
            }

            return ValidateJwsPayLoad(payload, "");
        }

        private bool ValidateJwsPayLoad(JwsPayload jwsPayload, string expectedIssuer)
        {
            var jwsIssuer = jwsPayload.GetIssuer();
            var jwsSubject = jwsPayload.GetClaimValue(SimpleIdServer.Jwt.Constants.UserClaims.Subject);
            var jwsAudiences = jwsPayload.GetAudiences();
            var expirationDateTime = jwsPayload.GetExpirationTime().ConvertFromUnixTimestamp();
            // 1. Check the client is correct.
            if (jwsSubject != jwsIssuer)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.BAD_CLIENT_ASSERTION_ISSUER);
            }

            // 2. Check if the audience is correct
            if (jwsAudiences == null || !jwsAudiences.Any() || !jwsAudiences.Any(j => j.Contains(expectedIssuer)))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.BAD_CLIENT_ASSERTION_AUDIENCES);
            }

            // 3. Check the expiration time
            if (DateTime.UtcNow > expirationDateTime)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.BAD_CLIENT_ASSERTION_EXPIRED);
            }

            return true;
        }
    }
}
