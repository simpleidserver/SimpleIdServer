// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSCIMRepresentationCommandRepository : InMemoryCommandRepository<SCIMRepresentation>, ISCIMRepresentationCommandRepository
    {
        public DefaultSCIMRepresentationCommandRepository(List<SCIMRepresentation> lstData) : base(lstData) { }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds)
        {
            IEnumerable<SCIMRepresentation> representations = LstData.AsQueryable().Where(r => representationIds.Contains(r.Id));
            return Task.FromResult(representations);
        }

        public IEnumerable<IEnumerable<SCIMRepresentation>> FindPaginatedRepresentations(IEnumerable<string> representationIds, string resourceType = null, int nbRecords = 50, bool ignoreAttributes = false)
        {
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var result = LstData.Where(r => filter.Contains(r.Id));
                if (!string.IsNullOrWhiteSpace(resourceType))
                    yield return result.Where(r => r.ResourceType == resourceType);
                else yield return result;
            }
        }

        public IEnumerable<IEnumerable<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var allAttributes = LstData.SelectMany(r => r.FlatAttributes);
            var query = allAttributes
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId);
            var nb = query.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var parentIds = query.Skip(i * nbRecords).Take(nbRecords);
                var result = allAttributes
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId));
                yield return result;
            }
        }


        public IEnumerable<IEnumerable<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var allAttributes = LstData.SelectMany(r => r.FlatAttributes);
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var parentIds = allAttributes
                    .Where(a => a.SchemaAttributeId == schemaAttributeId && filter.Contains(a.RepresentationId) && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                    .Select(r => r.ParentAttributeId)
                    .ToList();
                var result = allAttributes
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId));
                yield return result;
            }
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attrSchemaId, string value, string endpoint = null)
        {
            var result = LstData.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.FlatAttributes.Any(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueString == value));
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((SCIMRepresentation)result.Clone());
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attrSchemaId, int value, string endpoint = null)
        {
            var result = LstData.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.FlatAttributes.Any(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueInteger == value));
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult(result);
        }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType)
        {
            var result = LstData.Where(r =>
            {
                return r.ResourceType == resourceType && r.FlatAttributes.Any(a => a.FullPath == fullPath && values.Contains(a.ValueString));
            });
            return Task.FromResult(result);
        }

        public override Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = LstData.First(l => l.Id == data.Id);
            LstData.Remove(record);
            LstData.Add((SCIMRepresentation)data.Clone());
            return Task.FromResult(true);
        }

        public Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            foreach(var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var representation = LstData.Single(r => r.Id == scimRepresentationAttr.RepresentationId);
                representation.FlatAttributes.Add(scimRepresentationAttr);
            }

            return Task.CompletedTask;
        }

        public Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            foreach (var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var representation = LstData.Single(r => r.Id == scimRepresentationAttr.RepresentationId);
                var attr = representation.FlatAttributes.Single(r => r.Id == scimRepresentationAttr.Id);
                representation.FlatAttributes.Remove(attr);
            }

            return Task.CompletedTask;
        }

        public Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            foreach (var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var representation = LstData.Single(r => r.Id == scimRepresentationAttr.RepresentationId);
                var attr = representation.FlatAttributes.Single(r => r.Id == scimRepresentationAttr.Id);
                representation.FlatAttributes.Remove(attr);
                representation.FlatAttributes.Add(scimRepresentationAttr);
            }

            return Task.CompletedTask;
        }
    }
}
