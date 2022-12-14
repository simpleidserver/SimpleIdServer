// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientPrivateKeyJwtAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        private readonly IClientHelper _clientHelper;

        public OAuthClientPrivateKeyJwtAuthenticationHandler(IClientHelper clientHelper)
        {
            _clientHelper = clientHelper;
        }

        public string AuthMethod => "private_key_jwt";

        public async Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
            if (client == null) throw new ArgumentNullException(nameof(client));

            var handler = new JsonWebTokenHandler();
            var clientAssertion = authenticateInstruction.ClientAssertion;
            if (!handler.CanReadToken(clientAssertion)) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
            var token = handler.ReadJsonWebToken(clientAssertion);
            if (!token.IsSigned) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);

            var jsonWebKey = await _clientHelper.ResolveJsonWebKey(client, token.Kid, cancellationToken);
            var validationResult = handler.ValidateToken(clientAssertion, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                IssuerSigningKey = jsonWebKey
            });
            if (!validationResult.IsValid) throw new OAuthException(errorCode, validationResult.Exception.ToString());
            return ValidateJwsPayLoad(token, expectedIssuer, errorCode);
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
