// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultProvisioningStagingStore : IProvisioningStagingStore
{
    private readonly List<ExtractedRepresentationStaging> _extractedRepresentationStagings;

    public DefaultProvisioningStagingStore(List<ExtractedRepresentationStaging> extractedRepresentationStagings)
    {
        _extractedRepresentationStagings = extractedRepresentationStagings;
    }

    public Task<List<ExtractedRepresentation>> GetLastExtractedRepresentations(IEnumerable<string> externalIds, CancellationToken cancellationToken)
    {
        var result = _extractedRepresentationStagings
            .Where(s => externalIds.Contains(s.RepresentationId))
            .Select(s => new ExtractedRepresentation
            {
                ExternalId = s.RepresentationId,
                Version = s.RepresentationVersion
            })
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<ExtractedRepresentationStaging>> GetStagingExtractedRepresentations(string processId, int page, int count, CancellationToken cancellationToken)
    {
        var result = _extractedRepresentationStagings
            .Where(s => s.IdProvisioningProcessId == processId)
            .OrderBy(s => s.Id)
            .Skip(page * count)
            .Take(count)
            .ToList();
        return Task.FromResult(result);
    }

    public Task BulkUpdate(List<ExtractedRepresentation> extractedRepresentations, CancellationToken cancellationToken)
    {
        foreach (var rep in extractedRepresentations)
        {
            var staging = _extractedRepresentationStagings.FirstOrDefault(s => s.RepresentationId == rep.ExternalId);
            if (staging != null)
            {
                staging.RepresentationVersion = rep.Version;
            }
            else
            {
                _extractedRepresentationStagings.Add(new ExtractedRepresentationStaging
                {
                    Id = rep.ExternalId,
                    RepresentationId = rep.ExternalId,
                    RepresentationVersion = rep.Version,
                    IdProvisioningProcessId = string.Empty,
                    GroupIds = new List<string>(),
                    Type = default
                });
            }
        }
        return Task.CompletedTask;
    }

    public Task BulkUpdate(List<ExtractedRepresentationStaging> representations, CancellationToken cancellationToken)
    {
        foreach (var rep in representations)
        {
            var index = _extractedRepresentationStagings.FindIndex(s => s.Id == rep.Id);
            if (index >= 0)
            {
                _extractedRepresentationStagings[index] = rep;
            }
            else
            {
                _extractedRepresentationStagings.Add(rep);
            }
        }
        return Task.CompletedTask;
    }

    public Task<int> NbStagingExtractedRepresentations(string processId, CancellationToken cancellationToken)
    {
        var count = _extractedRepresentationStagings.Count(s => s.IdProvisioningProcessId == processId);
        return Task.FromResult(count);
    }
}
