// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IImportSummaryStore
    {
        IQueryable<ImportSummary> Query();
        void Add(ImportSummary importSummary);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class ImportSummaryStore : IImportSummaryStore
    {
        private readonly StoreDbContext _dbContext;

        public ImportSummaryStore(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ImportSummary> Query() => _dbContext.ImportSummaries;

        public void Add(ImportSummary importSummary) => _dbContext.ImportSummaries.Add(importSummary);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
