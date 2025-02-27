// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Realms;

public class RemoveRealmCommandConsumer : 
    IConsumer<RemoveRealmCommand>
{
    private readonly IUserSessionResitory _userSessionRepository;
    private readonly ISessionHelper _sessionHelper;
    private readonly IKeyStore _keyStore;
    private readonly IClientRepository _clientRepository;
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IRealmRepository _realmRepository;
    private readonly IdServerHostOptions _options;
    public static string Queuename = "remove-realm";

    public RemoveRealmCommandConsumer(
        IUserSessionResitory userSessionRepository,
        ISessionHelper sessionHelper,
        IKeyStore keyStore,
        IClientRepository clientRepository,
        IAuthenticationHelper authenticationHelper,
        ITransactionBuilder transactionBuilder,
        IRealmRepository realmRepository,
        IOptions<IdServerHostOptions> options)
    {
        _userSessionRepository = userSessionRepository;
        _sessionHelper = sessionHelper;
        _keyStore = keyStore;
        _clientRepository = clientRepository;
        _authenticationHelper = authenticationHelper;
        _transactionBuilder = transactionBuilder;
        _realmRepository = realmRepository;
        _options = options.Value;
    }

    public async Task Consume(ConsumeContext<RemoveRealmCommand> context)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var realm = await _realmRepository.Get(context.Message.Realm, context.CancellationToken);
            await RevokeRealmUserSessions(context.Message.Realm, context.CancellationToken);
            _realmRepository.Remove(realm);
            await transaction.Commit(context.CancellationToken);
        }
    }

    private async Task RevokeRealmUserSessions(string realm, CancellationToken cancellationToken)
    {
        const int nbSessions = 500;
        var nbActiveSessions = await _userSessionRepository.NbActiveSessions(realm, cancellationToken);
        var nbPage = Math.Ceiling((double)nbActiveSessions / nbSessions);
        var sigCredentials = _keyStore.GetAllSigningKeys(realm);
        for (var i = 0; i < nbPage; i++)
        {
            var activeSessions = await _userSessionRepository.SearchActiveSessions(realm, new Helpers.SearchRequest
            {
                Skip = (i * nbSessions),
                Take = nbSessions
            }, cancellationToken);
            foreach (var activeSession in activeSessions.Content)
            {
                var clientIds = activeSession.ClientIds
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct();
                var sub = _authenticationHelper.GetLogin(activeSession.User);
                var targetedClients = await _clientRepository.GetByClientIdsAndExistingBackchannelLogoutUri(realm, clientIds.ToList(), CancellationToken.None);
                var sessionClients = targetedClients.Where(c => activeSession.ClientIds.Contains(c.ClientId));
                activeSession.State = UserSessionStates.Rejected;
                _userSessionRepository.Update(activeSession);
                await _sessionHelper.RevokeBackChannels(sub, sigCredentials, activeSession, sessionClients, $"{_options.Authority}/{activeSession.Realm}", CancellationToken.None);
            }
        }
    }
}
