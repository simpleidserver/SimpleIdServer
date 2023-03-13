// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IFileSerializedKeyStore
    {
        IQueryable<SerializedFileKey> Query();
        void Add(SerializedFileKey key);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class SerializedFileKeyStore : IFileSerializedKeyStore
    {
        private readonly StoreDbContext _dbContext;

        public SerializedFileKeyStore(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<SerializedFileKey> Query() => _dbContext.SerializedFileKeys;

        public void Add(SerializedFileKey key) => _dbContext.SerializedFileKeys.Add(key);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
