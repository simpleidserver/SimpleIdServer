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

        public override Task<SCIMRepresentation> Get(string id, CancellationToken token)
        {
            var result = LstData.FirstOrDefault(r => r.Id == id);
            if (result == null) return Task.FromResult((SCIMRepresentation)null);
            return Task.FromResult(Enrich(result));
        }

        public override Task<bool> Add(SCIMRepresentation data, CancellationToken token)
        {
            foreach (var attr in data.FlatAttributes) attr.RepresentationId = data.Id;
            _attributes.AddRange(data.FlatAttributes);
            data.FlatAttributes.Clear();
            LstData.Add((SCIMRepresentation)data.Clone());
            return Task.FromResult(true);
        }

        public override Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = LstData.First(l => l.Id == data.Id);
            LstData.Remove(record);
            LstData.Add((SCIMRepresentation)data.Clone());
            return Task.FromResult(true);
        }

        public override Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            LstData.Remove(LstData.First(l => l.Equals(data)));
            _attributes.RemoveAll(a => a.RepresentationId == data.Id);
            return Task.FromResult(true);
        }

        public Task<List<SCIMRepresentation>> FindRepresentations(List<string> representationIds, string resourceType = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var representations = LstData.AsQueryable().Where(r => representationIds.Contains(r.Id));
            if (!string.IsNullOrWhiteSpace(resourceType)) representations = representations.Where(r => r.ResourceType == resourceType);
            return Task.FromResult(representations.Select(r => Enrich(r)).ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _attributes
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId);
            var parentIds = query.ToList();
            var result = _attributes
                .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                .ToList();
            return Task.FromResult(result);
        }
        
        public Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var parentIds = _attributes
                .Where(a => a.SchemaAttributeId == schemaAttributeId && representationIds.Contains(a.RepresentationId) && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .Select(r => r.ParentAttributeId)
                .Distinct()
                .ToList();
            var result = _attributes
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                    .ToList();
            return Task.FromResult(result);
        }

        public Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(string representationId, string schemaAttributeId, CancellationToken cancellationToken)
        {
            var query = _attributes
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.RepresentationId == representationId)
                .OrderBy(r => r.Id);
            var ids = query.Select(p => p.Id).Where(p => p != null);
            if (!ids.Any()) return Task.FromResult(query.ToList());
            var result = _attributes
                    .Where(a => ids.Contains(a.Id) || ids.Contains(a.ParentAttributeId)).ToList();
            return Task.FromResult(result);
        }

        public Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(List<string> representationIds, string schemaAttributeId, CancellationToken cancellationToken)
        {
            var query = _attributes
                .Where(a => a.SchemaAttributeId == schemaAttributeId && representationIds.Contains(a.RepresentationId))
                .OrderBy(r => r.Id);
            var ids = query.Select(p => p.Id).Where(p => p != null);
            if (!ids.Any()) return Task.FromResult(query.ToList());
            var result = _attributes
                    .Where(a => ids.Contains(a.Id) || ids.Contains(a.ParentAttributeId)).ToList();
            return Task.FromResult(result);
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression pathExpression, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => a.RepresentationId == representationId);
            var hierarchicalRepresentationAttributes = SCIMRepresentation.BuildHierarchicalAttributes(representationAttributes).AsQueryable();
            var allAttributes = new List<SCIMRepresentationAttribute>();
            var filteredAttributes = pathExpression.EvaluateAttributes(hierarchicalRepresentationAttributes, true).ToList();
            allAttributes.AddRange(filteredAttributes);
            foreach (var fAttr in filteredAttributes) ResolveChildren(representationAttributes.AsQueryable(), fAttr.Id, allAttributes);
            return Task.FromResult(allAttributes);
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => a.RepresentationId == representationId).ToList();
            return Task.FromResult(representationAttributes);
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByAproximativeFullPath(string representationId, string fullPath, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => a.RepresentationId == representationId && a.FullPath.StartsWith(fullPath));
            return Task.FromResult(representationAttributes.ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndValues(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => values.Contains(a.ValueString) && a.FullPath == fullPath);
            return Task.FromResult(representationAttributes.ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndRepresentationIds(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => values.Contains(a.RepresentationId) && a.FullPath == fullPath);
            return Task.FromResult(representationAttributes.ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesBySchemaAttributeAndValues(string schemaAttributeId, IEnumerable<string> values, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => values.Contains(a.ValueString) && a.SchemaAttributeId == schemaAttributeId);
            return Task.FromResult(representationAttributes.ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByReference(List<string> representationIds, string schemaAttributeId, string value, CancellationToken cancellationToken)
        {
            var representationAttributes = _attributes.Where(a => representationIds.Contains(a.RepresentationId) && a.SchemaAttributeId == schemaAttributeId && a.ValueString == value);
            return Task.FromResult(representationAttributes.ToList());
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, string value)
        {
            var result = _attributes.Where(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueString == value).ToList();
            return Task.FromResult(result);
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, int value)
        {
            var result = _attributes.Where(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueInteger == value).ToList();
            return Task.FromResult(result);
        }

        public Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId, bool isReference = false)
        {
            foreach(var scimRepresentationAttr in scimRepresentationAttributes)
                _attributes.Add(scimRepresentationAttr);
            return Task.CompletedTask;
        }

        public Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId, bool isReference = false)
        {
            foreach (var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var attr = _attributes.SingleOrDefault(r => r.Id == scimRepresentationAttr.Id);
                if (attr == null) continue;
                _attributes.Remove(attr);
            }

            return Task.CompletedTask;
        }

        public Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, bool isReference = false)
        {
            foreach (var scimRepresentationAttr in scimRepresentationAttributes)
            {
                var attr = _attributes.SingleOrDefault(r => r.Id == scimRepresentationAttr.Id);
                if (attr == null) continue;
                _attributes.Remove(attr);
                _attributes.Add(scimRepresentationAttr);
            }

            return Task.CompletedTask;
        }

        public override Task<ITransaction> StartTransaction(CancellationToken token) => Task.FromResult((ITransaction)new SCIMRepresentationTransaction(_attributes));

        private void ResolveChildren(IQueryable<SCIMRepresentationAttribute> representationAttributes, string parentId, List<SCIMRepresentationAttribute> children)
        {
            var filteredAttributes = representationAttributes.Where(a => a.ParentAttributeId == parentId);
            children.AddRange(filteredAttributes);
            foreach (var fAttr in filteredAttributes) ResolveChildren(representationAttributes, fAttr.Id, children);
        }

        private SCIMRepresentation Enrich(SCIMRepresentation representation)
        {
            var clone = (SCIMRepresentation)representation.Clone();
            clone.FlatAttributes = _attributes.Where(a => a.RepresentationId == representation.Id).ToList();
            return clone;
        }
    }

    public class SCIMRepresentationTransaction : ITransaction
    {
        private readonly List<SCIMRepresentationAttribute> _attributes;
        public SCIMRepresentationTransaction(List<SCIMRepresentationAttribute> attributes)
        {
            _attributes = attributes;
        }

        public Task Commit(CancellationToken token = default)
        {
            foreach (var attr in _attributes) attr.CachedChildren.Clear();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}
