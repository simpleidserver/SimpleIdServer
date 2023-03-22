// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Authenticate.AssertionParsers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Authenticate
{
    public interface IAuthenticateClient
    {
        Task<Client> Authenticate(AuthenticateInstruction authenticateIsnstruction, string issuerName, CancellationToken cancellationToken, bool isAuthorizationCodeGrantType = false, string errorCode = ErrorCodes.INVALID_CLIENT);
        bool TryGetClientId(AuthenticateInstruction instruction, out string clientId);

    }

    public class AuthenticateClient : IAuthenticateClient
    {
        private readonly IClientRepository _clientRepository;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _handlers;
        private readonly IEnumerable<IClientAssertionParser> _clientAssertionParsers;
        private readonly IBusControl _busControl;
        private readonly IdServerHostOptions _options;

        public AuthenticateClient(IClientRepository clientRepository, IEnumerable<IOAuthClientAuthenticationHandler> handlers, IEnumerable<IClientAssertionParser> clientAssertionParsers, IBusControl busControl, IOptions<IdServerHostOptions> options)
        {
            _clientRepository = clientRepository;
            _handlers = handlers;
            _clientAssertionParsers = clientAssertionParsers;
            _busControl = busControl;
            _options = options.Value;
        }

        public async Task<Client> Authenticate(AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, bool isAuthorizationCodeGrantType = false, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));

            string clientId;
            if (!TryGetClientId(authenticateInstruction, out clientId)) throw new OAuthException(errorCode, ErrorMessages.MISSING_CLIENT_ID);

            var client = await _clientRepository.Query()
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Scopes)
                .Include(c=> c.Realms)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == authenticateInstruction.Realm), cancellationToken);
            if (client == null) throw new OAuthException(errorCode, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            if (isAuthorizationCodeGrantType) return client;

            var tokenEndPointAuthMethod = client.TokenEndPointAuthMethod ?? _options.DefaultTokenEndPointAuthMethod;
            var handler = _handlers.FirstOrDefault(h => h.AuthMethod == tokenEndPointAuthMethod);
            if (handler == null) throw new OAuthException(errorCode, string.Format(ErrorMessages.UNKNOWN_AUTH_METHOD, tokenEndPointAuthMethod));

            if (!await handler.Handle(authenticateInstruction, client, issuerName, cancellationToken, errorCode))
            {
                await _busControl.Publish(new ClientAuthenticationFailureEvent
                {
                    ClientId = client.ClientId,
                    AuthMethod = tokenEndPointAuthMethod,
                    Realm = authenticateInstruction.Realm
                });
                throw new OAuthException(errorCode, ErrorMessages.BAD_CLIENT_CREDENTIAL);
            }

            await _busControl.Publish(new ClientAuthenticationSuccessEvent
            {
                ClientId = client.ClientId,
                AuthMethod = tokenEndPointAuthMethod,
                Realm = authenticateInstruction.Realm
            });
            return client;
        }

        public bool TryExtractClientIdFromClientAssertion(AuthenticateInstruction instruction, out string clientId)
        {
            clientId = null;
            if (string.IsNullOrWhiteSpace(instruction.ClientAssertionType)) return false;
            var type = instruction.ClientAssertionType;
            var parser = _clientAssertionParsers.FirstOrDefault(c => c.Type == type);
            if (parser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.CLIENT_ASSERTION_TYPE_NOT_SUPPORTED, type));
            var clientAssertion = instruction.ClientAssertion;
            if (string.IsNullOrWhiteSpace(clientAssertion)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_ASSERTION_IS_MISSING);
            if (!parser.TryExtractClientId(clientAssertion, out clientId)) return false;
            if (string.IsNullOrWhiteSpace(clientId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_ID_CANNOT_BE_EXTRACTED_FROM_CLIENT_ASSERTION);
            return true;
        }

        public bool TryGetClientId(AuthenticateInstruction instruction, out string clientId)
        {
            clientId = null;
            if (TryExtractClientIdFromClientAssertion(instruction, out clientId)) return true;

            clientId = instruction.ClientIdFromAuthorizationHeader;
            if (!string.IsNullOrWhiteSpace(clientId)) return true;
            
            clientId = instruction.ClientIdFromHttpRequestBody;
            if (!string.IsNullOrWhiteSpace(clientId)) return true;
            return false;
        }
    }
}
