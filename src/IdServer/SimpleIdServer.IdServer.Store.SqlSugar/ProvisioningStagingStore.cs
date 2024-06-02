// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class ProvisioningStagingStore : IProvisioningStagingStore
{
    private readonly DbContext _dbContext;

    public ProvisioningStagingStore(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BulkUpdate(List<ExtractedRepresentation> representations, CancellationToken cancellationToken)
    {
        var grps = representations.Select(g => Transform(g)).ToList();
        await _dbContext.Client.Fastest<SugarExtractedRepresentation>().BulkUpdateAsync(grps);
    }

    public async Task BulkUpdate(List<ExtractedRepresentationStaging> representations, CancellationToken cancellationToken)
    {
        var grps = representations.Select(g => Transform(g)).ToList();
        await _dbContext.Client.Fastest<SugarExtractedRepresentationStaging>().BulkUpdateAsync(grps);
    }

    public async Task<List<ExtractedRepresentation>> GetLastExtractedRepresentations(IEnumerable<string> externalIds, CancellationToken cancellationToken)
    {
        var representations = await _dbContext.Client.Queryable<SugarExtractedRepresentation>()
            .Where(r => externalIds.Contains(r.ExternalId))
            .ToListAsync();
        return representations.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<ExtractedRepresentationStaging>> GetStagingExtractedRepresentations(string processId, int page, int count, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarExtractedRepresentationStaging>()
            .OrderBy(s => s.Id)
            .Where(r => r.IdProvisioningProcessId == processId)
            .Skip(page * count)
            .Take(count)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<int> NbStagingExtractedRepresentations(string processId, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarExtractedRepresentationStaging>()
            .CountAsync(r => r.IdProvisioningProcessId == processId);
        return result;
    }

    private static SugarExtractedRepresentation Transform(ExtractedRepresentation representation)
    {
        return new SugarExtractedRepresentation
        {
            ExternalId = representation.ExternalId,
            Version = representation.Version
        };
    }

    private static SugarExtractedRepresentationStaging Transform(ExtractedRepresentationStaging representation)
    {
        return new SugarExtractedRepresentationStaging
        {
            GroupIds = representation.GroupIds == null ? string.Empty : string.Join(",", representation.GroupIds),
            Id = representation.Id,
            IdProvisioningProcessId = representation.IdProvisioningProcessId,
            RepresentationId = representation.RepresentationId,
            RepresentationVersion = representation.RepresentationVersion,
            Type = representation.Type,
            Values = representation.Values
        };
    }
}
