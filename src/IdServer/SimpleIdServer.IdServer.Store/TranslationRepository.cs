// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface ITranslationRepository
    {
        IQueryable<Translation> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class TranslationRepository : ITranslationRepository
    {
        private readonly StoreDbContext _dbContext;

        public TranslationRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Translation> Query() => _dbContext.Translations;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
