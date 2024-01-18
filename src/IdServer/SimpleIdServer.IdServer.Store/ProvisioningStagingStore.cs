// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface IProvisioningStagingStore
{
    Task<List<ExtractedRepresentation>> GetLastExtractedRepresentations(IEnumerable<string> externalIds, CancellationToken cancellationToken);
    Task<List<ExtractedRepresentationStaging>> GetStagingExtractedRepresentations(string processId, int page, int count, CancellationToken cancellationToken);
    Task BulkUpdate(List<ExtractedRepresentation> representations, CancellationToken cancellationToken);
    Task BulkUpdate(List<ExtractedRepresentationStaging> representations, CancellationToken cancellationToken);
    Task<int> NbStagingExtractedRepresentations(string processId, CancellationToken cancellationToken);
}

public class ProvisioningStagingStore : IProvisioningStagingStore
{
    private readonly StoreDbContext _dbContext;

    public ProvisioningStagingStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExtractedRepresentation>> GetLastExtractedRepresentations(IEnumerable<string> externalIds, CancellationToken cancellationToken)
    {
        if (_dbContext.Database.IsRelational())
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentation.ExternalId) }
            };
            var result = externalIds.Select(id => new ExtractedRepresentation { ExternalId = id }).ToList();
            await _dbContext.BulkReadAsync(result, bulkConfig, cancellationToken: cancellationToken);
            return result;
        }

        var representations = await _dbContext.ExtractedRepresentations
            .Where(r => externalIds.Contains(r.ExternalId))
            .ToListAsync();
        return representations;
    }

    public async Task<List<ExtractedRepresentationStaging>> GetStagingExtractedRepresentations(string processId, int page, int count, CancellationToken cancellationToken)
    {
        var result = await _dbContext.ExtractedRepresentationsStaging.OrderBy(s => s.Id)
            .Where(r => r.IdProvisioningProcessId == processId)
            .Skip(page * count)
            .Take(count)
            .ToListAsync(cancellationToken);
        return result;
    }

    public async Task BulkUpdate(List<ExtractedRepresentation> extractedRepresentations, CancellationToken cancellationToken)
    {
        if (_dbContext.Database.IsRelational())
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentation.ExternalId), nameof(ExtractedRepresentation.Version) }
            };
            await _dbContext.BulkInsertOrUpdateAsync(extractedRepresentations, bulkConfig, cancellationToken: cancellationToken);
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

    public async Task BulkUpdate(List<ExtractedRepresentationStaging> representations, CancellationToken cancellationToken)
    {
        if (_dbContext.Database.IsRelational())
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(ExtractedRepresentationStaging.Id) }
            };
            await _dbContext.BulkInsertOrUpdateAsync(representations, bulkConfig, cancellationToken: cancellationToken);
            return;
        }

        var ids = representations.Select(r => r.Id);
        var existingRepresentations = await _dbContext.ExtractedRepresentationsStaging
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();
        _dbContext.ExtractedRepresentationsStaging.RemoveRange(existingRepresentations);
        _dbContext.ExtractedRepresentationsStaging.AddRange(representations);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> NbStagingExtractedRepresentations(string processId, CancellationToken cancellationToken)
    {
        var result = await _dbContext.ExtractedRepresentationsStaging.CountAsync(r => r.IdProvisioningProcessId == processId);
        return result;
    }
}
