// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Authenticate.AssertionParsers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Authenticate
{
    public interface IAuthenticateClient
    {
        Task<Client> Authenticate(AuthenticateInstruction authenticateIsnstruction, string issuerName, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT);
        Task Authenticate(Client client, AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT);
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

        public async Task<Client> Authenticate(AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
            string clientId;
            if (!TryGetClientId(authenticateInstruction, out clientId)) throw new OAuthException(errorCode, Global.MissingClientId);

            var client = await _clientRepository.GetByClientId(authenticateInstruction.Realm, clientId, cancellationToken);
            if (client == null) throw new OAuthException(errorCode, string.Format(Global.UnknownClient, clientId));
            await Authenticate(client, authenticateInstruction, issuerName, cancellationToken, errorCode);
            return client;
        }

        public async Task Authenticate(Client client, AuthenticateInstruction authenticateInstruction, string issuerName, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
        {
            using (var activity = Tracing.IdserverActivitySource.StartActivity("AuthenticateClient"))
            {
                if (authenticateInstruction == null) throw new ArgumentNullException(nameof(authenticateInstruction));
                if (client.IsPublic) return;
                activity?.SetTag(Tracing.IdserverTagNames.ClientId, client.Id);
                var tokenEndPointAuthMethod = client.TokenEndPointAuthMethod ?? _options.DefaultTokenEndPointAuthMethod;
                var handler = _handlers.FirstOrDefault(h => h.AuthMethod == tokenEndPointAuthMethod);
                if (handler == null) throw new OAuthException(errorCode, string.Format(Global.UnknownAuthMethod, tokenEndPointAuthMethod));

                if (!await handler.Handle(authenticateInstruction, client, issuerName, cancellationToken, errorCode))
                {
                    await _busControl.Publish(new ClientAuthenticationFailureEvent
                    {
                        ClientId = client.ClientId,
                        AuthMethod = tokenEndPointAuthMethod,
                        Realm = authenticateInstruction.Realm
                    });
                    throw new OAuthException(errorCode, Global.BadClientCredential);
                }

                await _busControl.Publish(new ClientAuthenticationSuccessEvent
                {
                    ClientId = client.ClientId,
                    AuthMethod = tokenEndPointAuthMethod,
                    Realm = authenticateInstruction.Realm
                });
            }
        }

        public bool TryExtractClientIdFromClientAssertion(AuthenticateInstruction instruction, out string clientId)
        {
            clientId = null;
            if (string.IsNullOrWhiteSpace(instruction.ClientAssertionType)) return false;
            var type = instruction.ClientAssertionType;
            var parser = _clientAssertionParsers.FirstOrDefault(c => c.Type == type);
            if (parser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.ClientAssertionTypeNotSupported, type));
            var clientAssertion = instruction.ClientAssertion;
            if (string.IsNullOrWhiteSpace(clientAssertion)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.ClientAssertionIsMissing);
            if (!parser.TryExtractClientId(clientAssertion, out clientId)) return false;
            if (string.IsNullOrWhiteSpace(clientId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.ClientIdCannotBeExtractedFromClientAssertion);
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
