// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
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
            if(_dbContext.Database.IsRelational())
            {
                var bulkConfig = new BulkConfig
                {
                    PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentation.ExternalId) }
                };
                var result = externalIds.Select(id => new ExtractedRepresentation { ExternalId = id }).ToList();
                await _dbContext.BulkReadAsync(result, bulkConfig);
                return result;
            }

            var representations = await _dbContext.ExtractedRepresentations
                .Where(r => externalIds.Contains(r.ExternalId))
                .ToListAsync();
            return representations;
        }

        public async Task BulkUpdate(List<ExtractedRepresentation> extractedRepresentations)
        {
            if(_dbContext.Database.IsRelational())
            {
                var bulkConfig = new BulkConfig
                {
                    PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentation.ExternalId), nameof(ExtractedRepresentation.Version) }
                };
                await _dbContext.BulkInsertOrUpdateAsync(extractedRepresentations, bulkConfig);
                return;
            }

            var externalIds = extractedRepresentations.Select(r => r.ExternalId);
            var existingRepresentations = await _dbContext.ExtractedRepresentations
                .Where(e => externalIds.Contains(e.ExternalId))
                .ToListAsync();
            _dbContext.ExtractedRepresentations.RemoveRange(existingRepresentations);
            _dbContext.ExtractedRepresentations.AddRange(extractedRepresentations);
            await _dbContext.SaveChangesAsync();
        }
    }
}
