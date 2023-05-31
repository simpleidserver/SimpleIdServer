// Copyright(c) SimpleIdServer.All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IIdentityDocumentConfigurationStore
    {
        IQueryable<NetworkConfiguration> Query();
        void Add(NetworkConfiguration networkConfiguration);
        void Remove(NetworkConfiguration networkConfiguration);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class IdentityDocumentConfigurationStore : IIdentityDocumentConfigurationStore
    {
        private readonly StoreDbContext _dbContext;

        public IdentityDocumentConfigurationStore(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<NetworkConfiguration> Query() => _dbContext.Networks.AsQueryable();

        public void Add(NetworkConfiguration networkConfiguration) => _dbContext.Networks.Add(networkConfiguration);

        public void Remove(NetworkConfiguration networkConfiguration) => _dbContext.Networks.Remove(networkConfiguration);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => Task.FromResult(1);
    }
}
