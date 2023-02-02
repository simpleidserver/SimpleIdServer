// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IClientRepository
    {
        IQueryable<Client> Query();
        void Delete(Client client);
        void Add(Client client);
        void DeleteRange(IEnumerable<Client> clients);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class ClientRepository : IClientRepository
    {
        private readonly StoreDbContext _dbContext;

        public ClientRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Client> Query() => _dbContext.Clients;

        public void Delete(Client client) => _dbContext.Clients.Remove(client);

        public void DeleteRange(IEnumerable<Client> clients) => _dbContext.Clients.RemoveRange(clients);

        public void Add(Client client) => _dbContext.Clients.Add(client);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
