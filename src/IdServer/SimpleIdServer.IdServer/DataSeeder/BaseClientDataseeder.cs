// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.DataSeeder;

public abstract class BaseClientDataseeder : BaseAfterDeploymentDataSeeder
{
    private readonly IRealmRepository _realmRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IClientRepository _clientRepository;

    public BaseClientDataseeder(
        IRealmRepository realmRepository,
        IScopeRepository scopeRepository,
        IClientRepository clientRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
        _realmRepository = realmRepository;
        _scopeRepository = scopeRepository;
        _clientRepository = clientRepository;
    }

    protected async Task<bool> TryAddClient(Client client, CancellationToken cancellationToken)
    {
        var existingClient = await _clientRepository.GetByClientId(Constants.DefaultRealm, client.ClientId, cancellationToken);
        var existingScopes = await _scopeRepository.GetByNames(Constants.DefaultRealm, client.Scopes.Select(s => s.Name).ToList(), cancellationToken);
        if (existingClient != null)
        {
            var unknownScopes = existingScopes.Where(es => !existingClient.Scopes.Any(s => es.Name == s.Name)).ToList();
            foreach(var unknownScope in unknownScopes)
            {
                existingClient.Scopes.Add(unknownScope);
            }

            return false;
        }

        var masterRealm = await _realmRepository.Get(Constants.DefaultRealm, cancellationToken);
        client.Realms = new List<Realm>
        {
            masterRealm
        };
        client.Scopes = existingScopes;
        _clientRepository.Add(client);
        return true;
    }
}
