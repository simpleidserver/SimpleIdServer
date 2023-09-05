// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMRepresentationQueryRepository
    {
        Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter, CancellationToken cancellationToken);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, CancellationToken cancellationToken);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType, CancellationToken cancellationToken);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType, GetSCIMResourceParameter parameter, CancellationToken cancellationToken);
    }
}