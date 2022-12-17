// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Exceptions;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    /// <summary>
    /// https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication
    /// </summary>
    public class OAuthClientSecretJwtAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        public string AuthMethod => AUTH_METHOD;
        public const string AUTH_METHOD = "client_secret_jwt";

        public Task<bool> Handle(AuthenticateInstruction authenticateInstruction, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(client.ClientSecret)) throw new OAuthException(errorCode, ErrorMessages.NO_CLIENT_SECRET);
            var clientAssertion = authenticateInstruction.ClientAssertion;
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(clientAssertion)) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
            var token = handler.ReadJsonWebToken(clientAssertion);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(client.ClientSecret));
            var validationResult = handler.ValidateToken(clientAssertion, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = securityKey
            });
            if (!validationResult.IsValid) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_ASSERTION_SIGNATURE);
            ValidateJwsPayLoad(token, expectedIssuer, errorCode);
            return Task.FromResult(true);
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
