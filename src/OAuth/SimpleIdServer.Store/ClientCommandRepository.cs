// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;

namespace SimpleIdServer.Store
{
    public interface IClientCommandRepository
    {
        IQueryable<Client> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class ClientCommandRepository : IClientCommandRepository
    {
        private readonly StoreCommandDbContext _dbContext;

        public ClientCommandRepository(StoreCommandDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Client> Query() => _dbContext.Clients;

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
