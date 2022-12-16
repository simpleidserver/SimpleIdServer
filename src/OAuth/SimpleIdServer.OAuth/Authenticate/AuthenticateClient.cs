// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate
{
    public interface IAuthenticateClient
    {
        Task<Client> Authenticate(AuthenticateInstruction authenticateIsnstruction, string issuerName, CancellationToken cancellationToken, bool isAuthorizationCodeGrantType = false, string errorCode = ErrorCodes.INVALID_CLIENT);
    }

    public class AuthenticateClient : IAuthenticateClient
    {
        private readonly IClientRepository _clientRepository;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _handlers;
        private readonly OAuthHostOptions _options;

        public AuthenticateClient(IClientRepository clientRepository, IEnumerable<IOAuthClientAuthenticationHandler> handlers, IOptions<OAuthHostOptions> options)
        {
            _clientRepository = clientRepository;
            _handlers = handlers;
            _options = options.Value;
        }

        public async Task<Client> Authenticate(AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, bool isAuthorizationCodeGrantType = false, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));

            var clientId = TryGettingClientId(authenticateInstruction);
            if(string.IsNullOrWhiteSpace(clientId)) throw new OAuthException(errorCode, ErrorMessages.MISSING_CLIENT_ID);

            var client = await _clientRepository.Query().Include(c => c.SerializedJsonWebKeys).Include(c => c.Scopes).FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
            if (client == null) throw new OAuthException(errorCode, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            if (isAuthorizationCodeGrantType) return client;

            var tokenEndPointAuthMethod = client.TokenEndPointAuthMethod ?? _options.DefaultTokenEndPointAuthMethod;
            var handler = _handlers.FirstOrDefault(h => h.AuthMethod == tokenEndPointAuthMethod);
            if (handler == null) throw new OAuthException(errorCode, string.Format(ErrorMessages.UNKNOWN_AUTH_METHOD, tokenEndPointAuthMethod));

            if (!await handler.Handle(authenticateInstruction, client, issuerName, cancellationToken, errorCode)) throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_CREDENTIAL);

            return client;
        }

        private string TryGettingClientId(AuthenticateInstruction instruction)
        {
            var clientId = GetClientIdFromClientAssertion(instruction);
            if (!string.IsNullOrWhiteSpace(clientId))
                return clientId;

            clientId = instruction.ClientIdFromAuthorizationHeader;
            if (!string.IsNullOrWhiteSpace(clientId))
                return clientId;
            
            return instruction.ClientIdFromHttpRequestBody;
        }

        public string GetClientIdFromClientAssertion(AuthenticateInstruction instruction)
        {
            if (instruction.ClientAssertionType != "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" || string.IsNullOrWhiteSpace(instruction.ClientAssertion))
                return string.Empty;

            var clientAssertion = instruction.ClientAssertion;
            var handler = new JsonWebTokenHandler();
            var canRead = handler.CanReadToken(clientAssertion);
            if (!canRead) return string.Empty;
            var token = handler.ReadJsonWebToken(clientAssertion);
            if (token.IsEncrypted && token.IsSigned) return string.Empty;
            if (token.IsEncrypted) return instruction.ClientIdFromHttpRequestBody;
            var payload = token.GetClaimJson();
            return payload.GetStr(OpenIdConnectParameterNames.Iss);
        }
    }
}
