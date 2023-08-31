// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMRepresentationCommandRepository : ICommandRepository<SCIMRepresentation>
    {
        Task<List<SCIMRepresentation>> FindPaginatedRepresentations(List<string> representationIds, string resourceType = null, int nbRecords = 50, bool ignoreAttributes = false);
        Task<List<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null);
        Task<List<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null);
        Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression attrExpression, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByValueIndex(string representationId, IEnumerable<string> indexValueLst, string schemaAttributeId, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByFullPath(string representationId, string fullPath, CancellationToken cancellationToken);
        Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, string value, string endpoint = null);
        Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, int value, string endpoint = null);
        Task<List<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType);
        Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes);
        Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes);
        Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes);
    }
}