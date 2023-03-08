// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IRealmRepository
    {
        IQueryable<Realm> Query();
        void Add(Realm realm);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class RealmRepository : IRealmRepository
    {
        private readonly StoreDbContext _dbContext;

        public RealmRepository(StoreDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public IQueryable<Realm> Query() => _dbContext.Realms;

        public void Add(Realm realm) =>_dbContext.Realms.Add(realm);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
