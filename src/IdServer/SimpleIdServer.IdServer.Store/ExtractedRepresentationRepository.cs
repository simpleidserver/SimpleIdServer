// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using EFCore.BulkExtensions;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IExtractedRepresentationRepository
    {
        IQueryable<ExtractedRepresentation> Query();
        Task<IEnumerable<ExtractedRepresentation>> BulkRead(IEnumerable<string> externalIds);
        Task BulkUpdate(List<ExtractedRepresentation> extractedRepresentations);
    }

    public class ExtractedRepresentationRepository : IExtractedRepresentationRepository
    {
        private readonly StoreDbContext _dbContext;

        public ExtractedRepresentationRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ExtractedRepresentation> Query() => _dbContext.ExtractedRepresentations;

        public async Task<IEnumerable<ExtractedRepresentation>> BulkRead(IEnumerable<string> externalIds)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentation.ExternalId) }
            };
            var result = externalIds.Select(id => new ExtractedRepresentation { ExternalId = id }).ToList();
            await _dbContext.BulkReadAsync(result, bulkConfig);
            return result;
        }

        public Task BulkUpdate(List<ExtractedRepresentation> extractedRepresentations)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentation.ExternalId), nameof(ExtractedRepresentation.Version) }
            };
            return _dbContext.BulkInsertOrUpdateAsync(extractedRepresentations, bulkConfig);
        }
    }
}
