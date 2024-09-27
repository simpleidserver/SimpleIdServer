// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class ClientRepository : IClientRepository
{
    private readonly StoreDbContext _dbContext;

    public ClientRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Client> GetById(string realm, string id, CancellationToken cancellationToken)
    {
        return _dbContext.Clients
                .Include(c => c.Scopes).ThenInclude(s => s.ClaimMappers)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Translations)
                .Include(c => c.Realms)
                .SingleOrDefaultAsync(c => c.Id == id && c.Realms.Any(r => r.Name == realm), cancellationToken);
    }

    public Task<Client> GetByClientId(string realm, string clientId, CancellationToken cancellationToken)
    {
        return _dbContext.Clients
                .Include(c => c.Scopes).ThenInclude(s => s.ClaimMappers)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Translations)
                .Include(c => c.Realms)
                .SingleOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == realm), cancellationToken);
    }

    public Task<List<Client>> GetByClientIds(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        return _dbContext.Clients
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Realms)
                .Include(c => c.Scopes)
                .Include(c => c.Translations)
                .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
    }

    public Task<List<Client>> GetByClientIdsAndExistingBackchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        return _dbContext.Clients
            .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm) && !string.IsNullOrWhiteSpace(c.BackChannelLogoutUri))
            .ToListAsync();
    }

    public Task<List<Client>> GetByClientIdsAndExistingFrontchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        return _dbContext.Clients
            .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm) && !string.IsNullOrWhiteSpace(c.FrontChannelLogoutUri))
            .ToListAsync();
    }

    public Task<List<Client>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = _dbContext.Clients
                .Include(c => c.Translations)
                .Include(p => p.Realms)
                .Include(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
        return result;
    }

    public Task<List<Client>> GetAll(string realm, List<string> clientIds, CancellationToken cancellationToken)
    {
        var result = _dbContext.Clients
                .Include(p => p.Realms)
                .Where(p => clientIds.Contains(p.ClientId) && p.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<SearchResult<Client>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Client> query = _dbContext.Clients
            .Include(c => c.Translations)
            .Include(p => p.Realms)
            .Include(p => p.Scopes)
            .Where(p => p.Realms.Any(r => r.Name == realm))
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(r => r.UpdateDateTime);

        var nb = query.Count();
        var clients = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<Client>
        {
            Count = nb,
            Content = clients
        };
    }

    public Task<int> NbClients(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.Clients.Include(c => c.Realms).CountAsync(c => c.Realms.Any(r => r.Name == realm), cancellationToken);
    }

    public void Delete(Client client) => _dbContext.Clients.Remove(client);

    public void DeleteRange(IEnumerable<Client> clients) => _dbContext.Clients.RemoveRange(clients);

    public void Add(Client client) => _dbContext.Clients.Add(client);

    public void Update(Client client)
    {

    }
}
