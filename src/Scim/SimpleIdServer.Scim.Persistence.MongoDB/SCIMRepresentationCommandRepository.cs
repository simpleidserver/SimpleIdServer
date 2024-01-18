// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Infrastructures;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
    {
        private List<string> _removedAttributeIds = new List<string>();
        private List<string> _addedAttributeIds = new List<string>();
        private readonly SCIMDbContext _scimDbContext;
        private readonly IMongoClient _mongoClient;
        private readonly MongoDbOptions _options;
        private IClientSessionHandle _session;

        public SCIMRepresentationCommandRepository(SCIMDbContext scimDbContext, IMongoClient mongoClient, IOptions<MongoDbOptions> options)
        {
            _scimDbContext = scimDbContext;
            _mongoClient = mongoClient;
            _options = options.Value;
        }

        public async Task<SCIMRepresentation> Get(string id, CancellationToken token = default)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == id).ToMongoFirstAsync();
            if (result == null)
                return null;

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public async Task<bool> Add(SCIMRepresentation representation, CancellationToken token)
        {
            var record = new SCIMRepresentationModel(representation, _options.CollectionSchemas, _options.CollectionRepresentationAttributes);
            foreach (var flatAttr in representation.FlatAttributes) flatAttr.RepresentationId = representation.Id;
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.InsertOneAsync(_session, record, null, token);
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(_session, representation.FlatAttributes, cancellationToken: token);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.InsertOneAsync(record, null, token);
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(representation.FlatAttributes, cancellationToken: token);
            }

            return true;
        }

        public async Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = new SCIMRepresentationModel(data, _options.CollectionSchemas, _options.CollectionRepresentationAttributes);
            data.FlatAttributes.Clear();
            foreach (var newId in _addedAttributeIds) record.AttributeRefs.Add(new CustomMongoDBRef(_options.CollectionRepresentationAttributes, newId));
            record.AttributeRefs = record.AttributeRefs.Where(r => !_removedAttributeIds.Contains(r.Id.AsString)).ToList();
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == data.Id, record);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == data.Id, record);
            }

            return true;
        }

        public async Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            var attributeIds = data.FlatAttributes.Select(a => a.Id);
            var filter = Builders<SCIMRepresentationAttribute>.Filter.In(a => a.Id, attributeIds);
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.DeleteOneAsync(_session, d => d.Id == data.Id, null, token);
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(_session, filter);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.DeleteOneAsync(d => d.Id == data.Id, null, token);
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(filter, token);
            }

            return true;
        }

        public async Task<List<SCIMRepresentation>> FindRepresentations(List<string> representationIds, string resourceType = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => representationIds.Contains(r.Id));
            if (!string.IsNullOrWhiteSpace(resourceType))
                query = query.Where(r => r.ResourceType == resourceType);
            var result = await query.ToMongoListAsync();
            var references = result.SelectMany(r => r.SchemaRefs).Distinct().ToList();
            var schemas = MongoDBEntity.GetReferences<SCIMSchema>(references, _scimDbContext.Database);
            foreach (var representation in result)
                representation.Schemas = schemas.Where(s => representation.SchemaRefs.Any(r => r.Id == s.Id)).ToList();
            return result.Cast<SCIMRepresentation>().ToList();
        }

        public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var attributes = new List<SCIMRepresentationAttribute>();
            var query = _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId);
            var parentIds = await query.ToMongoListAsync();
            var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                .ToMongoListAsync();
            return result;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var parentIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && representationIds.Contains(a.RepresentationId) && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .Select(r => r.ParentAttributeId)
                .ToMongoListAsync();
            var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                .ToMongoListAsync();
            return result;
        }



        public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(string representationId, string schemaAttributeId, CancellationToken cancellationToken)
        {
            var ids = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.RepresentationId == representationId)
                .OrderBy(r => r.Id)
                .Select(r => r.Id)
                .ToMongoListAsync();
            var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => ids.Contains(a.Id) || ids.Contains(a.ParentAttributeId))
                .ToMongoListAsync();
            return result;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(List<string> representationIds, string schemaAttributeId, CancellationToken cancellationToken)
        {
            var ids = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && representationIds.Contains(a.RepresentationId))
                .OrderBy(r => r.Id)
                .Select(r => r.Id)
                .ToMongoListAsync();
            var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => ids.Contains(a.Id) || ids.Contains(a.ParentAttributeId))
                .ToMongoListAsync();
            return result;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression pathExpression, CancellationToken cancellationToken)
        {
            if (pathExpression.SchemaAttribute == null || string.IsNullOrWhiteSpace(pathExpression.SchemaAttribute.Id)) return new List<SCIMRepresentationAttribute>();
            IMongoQueryable<EnrichedAttribute> representationAttributes = from a in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                join b in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable() on a.ParentAttributeId equals b.Id into Parents
                where a.RepresentationId == representationId
                select new EnrichedAttribute
                {
                    Attribute = a,
                    Parent = Parents.First(),
                    Children = new List<SCIMRepresentationAttribute>()
                };
            representationAttributes = pathExpression.EvaluateMongoDbAttributes(representationAttributes);
            var targetedAttributes = await representationAttributes.ToMongoListAsync();
            var allParentIds = targetedAttributes.Select(a => a.Attribute.ParentAttributeId);
            var allTargetedAttributeIds = targetedAttributes.Select(a => a.Attribute.Id);
            var allChildren = await (from a in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                           where a.ParentAttributeId != null && a.RepresentationId == representationId && (allParentIds.Contains(a.ParentAttributeId) || allTargetedAttributeIds.Contains(a.ParentAttributeId))
                           select a).ToMongoListAsync();

            var result = new List<SCIMRepresentationAttribute>();
            foreach (var targetedAttribute in targetedAttributes)
            {
                if (targetedAttribute.Parent != null) result.Add(targetedAttribute.Parent);
                result.Add(targetedAttribute.Attribute);
                var siblingAttributes = allChildren.Where(c => (targetedAttribute.Parent != null && c.ParentAttributeId == targetedAttribute.Parent.Id) || (targetedAttribute.Attribute.Id == c.ParentAttributeId)).Where(a => !result.Any(r => r.Id == a.Id));
                result.AddRange(siblingAttributes);
            }

            return result;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(r => r.RepresentationId == representationId)
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByAproximativeFullPath(string representationId, string fullPath, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.RepresentationId == representationId && a.FullPath.StartsWith(fullPath))
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndValues(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => values.Contains(a.ValueString) && a.FullPath == fullPath)
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndRepresentationIds(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => values.Contains(a.RepresentationId) && a.FullPath == fullPath)
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesBySchemaAttributeAndValues(string schemaAttributeId, IEnumerable<string> values, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => values.Contains(a.ValueString) && a.SchemaAttributeId == schemaAttributeId)
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByComputedValueIndexAndRepresentationId(List<string> computedValueIndexLst, string representationId, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => computedValueIndexLst.Contains(a.ComputedValueIndex) && a.RepresentationId == representationId)
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByReference(List<string> representationIds, string schemaAttributeId, string value, CancellationToken cancellationToken)
        {
            var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => representationIds.Contains(a.RepresentationId) && a.SchemaAttributeId == schemaAttributeId && a.ValueString == value)
                .ToMongoListAsync();
            return representationAttributes;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, string value)
        {
            var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueString == value)
                .ToMongoListAsync();
            return result;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, int value)
        {
            var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueInteger == value)
                .ToMongoListAsync();
            return result;
        }

        public async Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId = null, bool isReference = false)
        {
            if (!scimRepresentationAttributes.Any()) return;
            scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId));
            var representationIds = scimRepresentationAttributes.Select(r => r.RepresentationId).Distinct();
            List<SCIMRepresentationModel> result = null;
            if (_session == null)
                result = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();
            else
                result = await _scimDbContext.SCIMRepresentationLst.AsQueryable(_session).Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();

            _addedAttributeIds.AddRange(scimRepresentationAttributes.Where(a => a.RepresentationId == currentRepresentationId).Select(a => a.Id));
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(_session, scimRepresentationAttributes);
                if (isReference)
                {
                    foreach (var attrs in scimRepresentationAttributes.GroupBy(a => a.RepresentationId))
                    {
                        var currentRepresentation = result.Single(r => r.Id == attrs.Key);
                        foreach (var id in attrs.Select(a => a.Id)) currentRepresentation.AttributeRefs.Add(new CustomMongoDBRef(_options.CollectionRepresentationAttributes, id));
                        await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == currentRepresentation.Id, currentRepresentation);
                    }
                }
            }
            else
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(scimRepresentationAttributes);
                if(isReference)
                {
                    foreach (var attrs in scimRepresentationAttributes.GroupBy(a => a.RepresentationId))
                    {
                        var currentRepresentation = result.Single(r => r.Id == attrs.Key);
                        foreach (var id in attrs.Select(a => a.Id)) currentRepresentation.AttributeRefs.Add(new CustomMongoDBRef(_options.CollectionRepresentationAttributes, id));
                        await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == currentRepresentation.Id, currentRepresentation);
                    }
                }
            }
        }

        public async Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, bool isReference = false)
        {
            if (!scimRepresentationAttributes.Any()) return;
            scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId));
            if (_session != null)
            {
                foreach (var attr in scimRepresentationAttributes)
                    await _scimDbContext.SCIMRepresentationAttributeLst.ReplaceOneAsync(_session, s => s.Id == attr.Id, attr, new ReplaceOptions { IsUpsert = true });
            }
            else
            {
                foreach (var attr in scimRepresentationAttributes)
                    await _scimDbContext.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr, new ReplaceOptions { IsUpsert = true });
            }
        }

        public async Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId, bool isReference = false)
        {
            if (!scimRepresentationAttributes.Any()) return;
            scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId));
            var representationIds = scimRepresentationAttributes.Select(r => r.RepresentationId).Distinct();
            List<SCIMRepresentationModel> result = null;
            if (_session == null)
                result = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();
            else
                result = await _scimDbContext.SCIMRepresentationLst.AsQueryable(_session).Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();
            var attributeIds = scimRepresentationAttributes.Select(a => a.Id);
            var filter = Builders<SCIMRepresentationAttribute>.Filter.In(a => a.Id, attributeIds);
            _removedAttributeIds.AddRange(scimRepresentationAttributes.Where(r => r.RepresentationId == currentRepresentationId).Select(r => r.Id));
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(_session, filter);
                if (isReference)
                {
                    foreach (var attrs in scimRepresentationAttributes.GroupBy(a => a.RepresentationId))
                    {
                        var currentRepresentation = result.Single(r => r.Id == attrs.Key);
                        var attrIds = attrs.Select(a => a.Id);
                        currentRepresentation.AttributeRefs = currentRepresentation.AttributeRefs.Where(a => !attrIds.Contains(a.Id.AsString)).ToList();
                        await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == currentRepresentation.Id, currentRepresentation);
                    }
                }
            }
            else
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(filter);
                if (isReference)
                {
                    foreach (var attrs in scimRepresentationAttributes.GroupBy(a => a.RepresentationId))
                    {
                        var currentRepresentation = result.Single(r => r.Id == attrs.Key);
                        var attrIds = attrs.Select(a => a.Id);
                        currentRepresentation.AttributeRefs = currentRepresentation.AttributeRefs.Where(a => !attrIds.Contains(a.Id.AsString)).ToList();
                        await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == currentRepresentation.Id, currentRepresentation);
                    }
                }
            }
        }


        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            if (_options.SupportTransaction)
            {
                _session = await _mongoClient.StartSessionAsync(null, token);
                _session.StartTransaction();
                return new MongoDbTransaction(_session);
            }

            _session = null;
            return new MongoDbTransaction();
        }
    }
}
