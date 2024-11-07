// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class ClientRepository : IClientRepository
    {
        private readonly DbContext _dbContext;

        public ClientRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Client client)
        {
            _dbContext.Client.InsertNav(SugarClient.Transform(client))
                .Include(c => c.ClientScopes).ThenInclude(s => s.Scope)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Realms)
                .Include(c => c.DeviceAuthCodes)
                .Include(c => c.Translations)
                .ExecuteCommand();
        }

        public void Delete(Client client)
        {
            _dbContext.Client.Deleteable(SugarClient.Transform(client)).ExecuteCommand();
        }

        public void Update(Client client)
        {
            var transformedClient = SugarClient.Transform(client);
            _dbContext.Client.Updateable(transformedClient).ExecuteCommand();
            _dbContext.Client.UpdateNav(transformedClient)
                .Include(c => c.ClientScopes)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.Realms)
                .Include(c => c.DeviceAuthCodes)
                .Include(c => c.Translations)
                .ExecuteCommand();
        }

        public void DeleteRange(IEnumerable<Client> clients)
        {
            var cls = clients.Select(c => SugarClient.Transform(c)).ToList();
            _dbContext.Client.Deleteable(cls).ExecuteCommand();
        }

        public async Task<List<Client>> GetAll(string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Translations)
                .Includes(p => p.Realms)
                .Includes(p => p.ClientScopes, p => p.Scope)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<Client>> GetAll(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                    .Includes(p => p.Realms)
                    .Where(p => clientIds.Contains(p.ClientId) && p.Realms.Any(r => r.RealmsName == realm))
                    .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<Client> GetById(string realm, string id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.ClientScopes, c => c.Scope, s => s.ClaimMappers)
                .Includes(c => c.SerializedJsonWebKeys)
                .Includes(c => c.Translations)
                .Includes(c => c.Realms)
                .FirstAsync(c => c.Id == id && c.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result?.ToDomain();
        }

        public async Task<Client> GetByClientId(string realm, string clientId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.ClientScopes, c => c.Scope, s => s.ClaimMappers)
                .Includes(c => c.SerializedJsonWebKeys)
                .Includes(c => c.Translations)
                .Includes(c => c.Realms)
                .FirstAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<Client>> GetByClientIds(List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.SerializedJsonWebKeys)
                .Includes(c => c.Realms)
                .Includes(c => c.ClientScopes, c => c.Scope)
                .Includes(c => c.Translations)
                .Where(c => clientIds.Contains(c.ClientId))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<Client>> GetByClientIds(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.SerializedJsonWebKeys)
                .Includes(c => c.Realms)
                .Includes(c => c.ClientScopes, c => c.Scope)
                .Includes(c => c.Translations)
                .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList(); 
        }

        public async Task<List<Client>> GetByClientIdsAndExistingBackchannelLogoutUri(List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Where(c => clientIds.Contains(c.ClientId) && !string.IsNullOrWhiteSpace(c.BackChannelLogoutUri))
                .ToListAsync();
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<Client>> GetByClientIdsAndExistingFrontchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarClient>()
                .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.RealmsName == realm) && !string.IsNullOrWhiteSpace(c.FrontChannelLogoutUri))
                .ToListAsync();
            return result.Select(r => r.ToDomain()).ToList();
        }

        public Task<int> NbClients(string realm, CancellationToken cancellationToken)
        {
            return _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Realms)
                .CountAsync(c => c.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        }

        public async Task<SearchResult<Client>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
        {
            var result = _dbContext.Client.Queryable<SugarClient>()
                .Includes(c => c.Translations)
                .Includes(p => p.Realms)
                .Includes(p => p.ClientScopes, p => p.Scope)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm));
            result = result.OrderByDescending(c => c.UpdateDateTime);
            /*
            if (!string.IsNullOrWhiteSpace(request.Filter))
                result = result.Where(request.Filter);
            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                result = result.OrderBy(request.OrderBy);
            else
                result = result.OrderByDescending(r => r.UpdateDateTime);
            */

            var nb = result.Count();
            var clients = await result.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new SearchResult<Client>
            {
                Count = nb,
                Content = clients.Select(c => c.ToDomain()).ToList()
            };
        }
    }
}
