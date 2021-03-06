﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate
{
    public interface IAuthenticateClient
    {
        Task<BaseClient> Authenticate(AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, bool isAuthorizationCodeGrantType = false, string errorCode = ErrorCodes.INVALID_CLIENT);
    }

    public class AuthenticateClient : IAuthenticateClient
    {
        private readonly IJwsGenerator _jwsGenerator;
        private readonly IJwtParser _jwtParser;
        private readonly IOAuthClientRepository _oauthClientRepository;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _handlers;

        public AuthenticateClient(IJwsGenerator jwsGenerator, IJwtParser jwtParser, IOAuthClientRepository oauthClientRepository, IEnumerable<IOAuthClientAuthenticationHandler> handlers)
        {
            _jwsGenerator = jwsGenerator;
            _jwtParser = jwtParser;
            _oauthClientRepository = oauthClientRepository;
            _handlers = handlers;
        }

        public async Task<BaseClient> Authenticate(AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, bool isAuthorizationCodeGrantType = false, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null)
            {
                throw new ArgumentNullException(nameof(authenticateInstruction));
            }

            BaseClient client = null;
            var clientId = TryGettingClientId(authenticateInstruction);
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                client = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
            }

            if (client == null)
            {
                throw new OAuthException(errorCode, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            if (isAuthorizationCodeGrantType)
            {
                return client;
            }

            var tokenEndPointAuthMethod = client.TokenEndPointAuthMethod;
            var handler = _handlers.FirstOrDefault(h => h.AuthMethod == tokenEndPointAuthMethod);
            if (handler == null)
            {
                throw new OAuthException(errorCode, string.Format(ErrorMessages.UNKNOWN_AUTH_METHOD, tokenEndPointAuthMethod));
            }

            if (!await handler.Handle(authenticateInstruction, client, issuerName, cancellationToken, errorCode))
            {
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_CREDENTIAL);
            }

            return client;
        }

        private string TryGettingClientId(AuthenticateInstruction instruction)
        {
            var clientId = GetClientIdFromClientAssertion(instruction);
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                return clientId;
            }

            clientId = instruction.ClientIdFromAuthorizationHeader;
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                return clientId;
            }
            
            return instruction.ClientIdFromHttpRequestBody;
        }

        public string GetClientIdFromClientAssertion(AuthenticateInstruction instruction)
        {
            if (instruction.ClientAssertionType != "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" || string.IsNullOrWhiteSpace(instruction.ClientAssertion))
            {
                return string.Empty;
            }

            var clientAssertion = instruction.ClientAssertion;
            var isJweToken = _jwtParser.IsJweToken(clientAssertion);
            var isJwsToken = _jwtParser.IsJwsToken(clientAssertion);
            if (isJweToken && isJwsToken)
            {
                return string.Empty;
            }

            if (isJweToken)
            {
                return instruction.ClientIdFromHttpRequestBody;
            }

            var payload = _jwsGenerator.ExtractPayload(clientAssertion);
            if (payload == null)
            {
                return string.Empty;
            }

            return payload.GetClaimValue("iss");
        }
    }
}
