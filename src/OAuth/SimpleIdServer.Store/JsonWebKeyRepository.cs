// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;

namespace SimpleIdServer.Store
{
    public interface IJsonWebKeyRepository
    {
        IQueryable<JsonWebKey> Query();
        void Add(JsonWebKey key);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class JsonWebKeyRepository : IJsonWebKeyRepository
    {
        private readonly StoreDbContext _dbContext;

        public JsonWebKeyRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<JsonWebKey> Query() => _dbContext.JsonWebKeys;

        public void Add(JsonWebKey key) => _dbContext.JsonWebKeys.Add(key);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
