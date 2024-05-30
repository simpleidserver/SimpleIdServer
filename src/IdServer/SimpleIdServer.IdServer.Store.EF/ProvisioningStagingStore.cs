// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class ProvisioningStagingStore : IProvisioningStagingStore
{
    private readonly StoreDbContext _dbContext;

    public ProvisioningStagingStore(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExtractedRepresentation>> GetLastExtractedRepresentations(IEnumerable<string> externalIds, CancellationToken cancellationToken)
    {
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
            var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.InsertWhenNotMatched(
                            LinqToDB.LinqExtensions.On(
                                LinqToDB.LinqExtensions.Using(
                                    LinqToDB.LinqExtensions.Merge(
                                        _dbContext.ExtractedRepresentations.ToLinqToDBTable()),
                                        extractedRepresentations
                                    ),
                                    (g1, g2) => g1.ExternalId == g2.ExternalId
                            ),
                            source => source),
                        (target, source) => new ExtractedRepresentation
                        {
                            ExternalId = source.ExternalId,
                            Version = source.Version
                        });
            LinqToDB.LinqExtensions.Merge(merged);
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
            var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.InsertWhenNotMatched(
                            LinqToDB.LinqExtensions.On(
                                LinqToDB.LinqExtensions.Using(
                                    LinqToDB.LinqExtensions.Merge(
                                        _dbContext.ExtractedRepresentationsStaging.ToLinqToDBTable()),
                                        representations
                                    ),
                                    (g1, g2) => g1.Id == g2.Id
                            ),
                            source => source),
                        (target, source) => new ExtractedRepresentationStaging
                        {
                            GroupIds = source.GroupIds,
                            IdProvisioningProcessId = source.IdProvisioningProcessId,
                            RepresentationId = source.RepresentationId,
                            RepresentationVersion = source.RepresentationVersion,
                            Type = source.Type,
                            Values = source.Values
                        });
            LinqToDB.LinqExtensions.Merge(merged);
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
