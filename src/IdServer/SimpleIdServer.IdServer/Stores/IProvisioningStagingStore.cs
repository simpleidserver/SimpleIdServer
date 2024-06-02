// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IProvisioningStagingStore
{
    Task<List<ExtractedRepresentation>> GetLastExtractedRepresentations(IEnumerable<string> externalIds, CancellationToken cancellationToken);
    Task<List<ExtractedRepresentationStaging>> GetStagingExtractedRepresentations(string processId, int page, int count, CancellationToken cancellationToken);
    Task BulkUpdate(List<ExtractedRepresentation> representations, CancellationToken cancellationToken);
    Task BulkUpdate(List<ExtractedRepresentationStaging> representations, CancellationToken cancellationToken);
    Task<int> NbStagingExtractedRepresentations(string processId, CancellationToken cancellationToken);
}
