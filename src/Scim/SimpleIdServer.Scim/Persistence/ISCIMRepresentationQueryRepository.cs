// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMRepresentationQueryRepository
    {
        Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId);
        Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType);
    }
}