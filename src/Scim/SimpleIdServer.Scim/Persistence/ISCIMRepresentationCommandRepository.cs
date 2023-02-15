// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMRepresentationCommandRepository : ICommandRepository<SCIMRepresentation>
    {
        Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds);
        Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType);
        IEnumerable<(IEnumerable<SCIMRepresentation>, IEnumerable<string>)> FindPaginatedSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType = null, int nbRecords = 50);
        Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, string value, string endpoint = null);
        Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, int value, string endpoint = null);
        Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType);
        Task BulkUpdate(IEnumerable<SCIMRepresentation> scimRepresentations);
        Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes);
    }
}