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
        Task<SCIMRepresentation> GetWithAttributes(string id, CancellationToken token = default(CancellationToken));
        Task<List<SCIMRepresentation>> FindRepresentations(List<string> representationIds, string resourceType = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(string representationId, string schemaAttributeId, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(List<string> representationIds, string schemaAttributeId, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression attrExpression, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByAproximativeFullPath(string representationId, string fullPath, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndValues(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndRepresentationIds(string fullPath, IEnumerable<string> representationIds, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesBySchemaAttributeAndValues(string schemaAttributeId, IEnumerable<string> values, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByComputedValueIndexAndRepresentationId(List<string> computedValueIndexLst, string representationId, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByReference(List<string> representationIds, string schemaAttributeId, string value, CancellationToken cancellationToken);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, string value);
        Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, int value);
        Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId = null, bool isReference = false);
        Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId = null, bool isReference = false);
        Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, bool isReference = false);
    }
}