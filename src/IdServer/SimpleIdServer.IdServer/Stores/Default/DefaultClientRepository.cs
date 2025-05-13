// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultClientRepository : IClientRepository
{
    private readonly List<Client> _clients;

    public DefaultClientRepository(List<Client> clients)
    {
        _clients = clients;
    }

    public Task<Client> GetById(string realm, string id, CancellationToken cancellationToken)
    {
        var client = _clients.AsQueryable().SingleOrDefault(c => c.Id == id && c.Realms.Any(r => r.Name == realm));
        return Task.FromResult(client);
    }

    public Task<Client> GetByClientId(string realm, string clientId, CancellationToken cancellationToken)
    {
        var client = _clients.AsQueryable().SingleOrDefault(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == realm));
        return Task.FromResult(client);
    }

    public Task<List<Client>> GetByClientIds(List<string> clientIds, CancellationToken cancellationToken)
    {
        var clients = _clients.AsQueryable().Where(c => clientIds.Contains(c.ClientId)).ToList();
        return Task.FromResult(clients);
    }

    public Task<List<Client>> GetByClientIds(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        var clients = _clients.AsQueryable().Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm)).ToList();
        return Task.FromResult(clients);
    }

    public Task<List<Client>> GetByClientIdsAndExistingBackchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        var clients = _clients.AsQueryable().Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm) && !string.IsNullOrWhiteSpace(c.BackChannelLogoutUri)).ToList();
        return Task.FromResult(clients);
    }

    public Task<List<Client>> GetByClientIdsAndExistingFrontchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        var clients = _clients.AsQueryable().Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm) && !string.IsNullOrWhiteSpace(c.FrontChannelLogoutUri)).ToList();
        return Task.FromResult(clients);
    }

    public Task<List<Client>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var clients = _clients.AsQueryable().Where(c => c.Realms.Any(r => r.Name == realm)).ToList();
        return Task.FromResult(clients);
    }

    public Task<List<Client>> GetAll(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        var clients = _clients.AsQueryable().Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm)).ToList();
        return Task.FromResult(clients);
    }

    public async Task<SearchResult<Client>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _clients.AsQueryable().Where(c => c.Realms.Any(r => r.Name == realm));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(r => r.UpdateDateTime);
        var nb = query.Count();
        var clients = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        return await Task.FromResult(new SearchResult<Client>
        {
            Count = nb,
            Content = clients
        });
    }

    public Task<int> NbClients(string realm, CancellationToken cancellationToken)
    {
        var count = _clients.AsQueryable().Count(c => c.Realms.Any(r => r.Name == realm));
        return Task.FromResult(count);
    }

    public void Delete(Client client) => _clients.Remove(client);

    public void DeleteRange(IEnumerable<Client> clients) => _clients.RemoveAll(c => clients.Contains(c));

    public void Add(Client client) => _clients.Add(client);

    public void Update(Client client)
    {
    }

    public Task BulkAdd(List<Client> clients)
    {
        _clients.AddRange(clients);
        return Task.CompletedTask;
    }
}
