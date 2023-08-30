// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSCIMRepresentationCommandRepository : InMemoryCommandRepository<SCIMRepresentation>, ISCIMRepresentationCommandRepository
    {
        private readonly List<SCIMRepresentationAttribute> _attributes;

        public DefaultSCIMRepresentationCommandRepository(List<SCIMRepresentation> lstData, List<SCIMRepresentationAttribute> attributes) : base(lstData) 
        {
            _attributes = attributes;
        }

        public override Task<bool> Add(SCIMRepresentation data, CancellationToken token)
        {
            foreach (var attr in data.FlatAttributes) attr.RepresentationId = data.Id;
            _attributes.AddRange(data.FlatAttributes);
            data.FlatAttributes.Clear();
            LstData.Add((SCIMRepresentation)data.Clone());
            return Task.FromResult(true);
        }

        public Task<List<SCIMRepresentation>> FindPaginatedRepresentations(List<string> representationIds, string resourceType = null, int nbRecords = 50, bool ignoreAttributes = false)
        {
            var representations = LstData.AsQueryable().Where(r => representationIds.Contains(r.Id));
            if (!string.IsNullOrWhiteSpace(resourceType)) representations = representations.Where(r => r.ResourceType == resourceType);
            return Task.FromResult(representations.ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var allAttributes = LstData.SelectMany(r => r.FlatAttributes);
            var query = allAttributes
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId);
            var nb = query.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            var results = new List<SCIMRepresentationAttribute>();
            for (var i = 0; i <= nbPages; i++)
            {
                var parentIds = query.Skip(i * nbRecords).Take(nbRecords);
                var result = allAttributes
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId));
                results.AddRange(result);
            }
            return Task.FromResult(results);
        }



        public Task<List<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var allAttributes = LstData.SelectMany(r => r.FlatAttributes);
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            var results = new List<SCIMRepresentationAttribute>();
            for (var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var parentIds = allAttributes
                    .Where(a => a.SchemaAttributeId == schemaAttributeId && filter.Contains(a.RepresentationId) && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                    .Select(r => r.ParentAttributeId)
                    .ToList();
                var result = allAttributes
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId));
                results.AddRange(result);
            }
            return Task.FromResult(results);
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression pathExpression, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => a.RepresentationId == representationId);
            var hierarchicalRepresentationAttributes = SCIMRepresentation.BuildHierarchicalAttributes(representationAttributes).AsQueryable();
            var allAttributes = new List<SCIMRepresentationAttribute>();
            var filteredAttributes = pathExpression.EvaluateAttributes(hierarchicalRepresentationAttributes, true);
            allAttributes.AddRange(filteredAttributes);
            foreach (var fAttr in filteredAttributes) ResolveChildren(representationAttributes.AsQueryable(), fAttr.Id, allAttributes);
            return Task.FromResult(allAttributes);
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByValueIndex(string representationId, string indexValue, string schemaAttributeId, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => a.RepresentationId == representationId);
            return Task.FromResult(representationAttributes.Where(a => a.ComputedValueIndex == indexValue && a.SchemaAttributeId == schemaAttributeId).ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByFullPath(string representationId, string fullPath, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => a.RepresentationId == representationId && a.FullPath.StartsWith(fullPath));
            return Task.FromResult(representationAttributes.ToList());
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

        public Task<List<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType)
        {
            var result = LstData.Where(r =>
            {
                return r.ResourceType == resourceType && r.FlatAttributes.Any(a => a.FullPath == fullPath && values.Contains(a.ValueString));
            }).ToList();
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
                _attributes.Add(scimRepresentationAttr);
            }

            return Task.CompletedTask;
        }

        public Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            foreach (var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var attr = _attributes.Single(r => r.Id == scimRepresentationAttr.Id);
                _attributes.Remove(attr);
            }

            return Task.CompletedTask;
        }

        public Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            foreach (var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var attr = _attributes.Single(r => r.Id == scimRepresentationAttr.Id);
                _attributes.Remove(attr);
                _attributes.Add(scimRepresentationAttr);
            }

            return Task.CompletedTask;
        }

        private void ResolveChildren(IQueryable<SCIMRepresentationAttribute> representationAttributes, string parentId, List<SCIMRepresentationAttribute> children)
        {
            var filteredAttributes = representationAttributes.Where(a => a.ParentAttributeId == parentId);
            children.AddRange(filteredAttributes);
            foreach (var fAttr in filteredAttributes) ResolveChildren(representationAttributes, fAttr.Id, children);
        }
    }
}
