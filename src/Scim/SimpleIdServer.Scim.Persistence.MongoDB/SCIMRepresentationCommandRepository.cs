// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
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

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds)
        {
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => representationIds.Contains(r.Id))
                .ToMongoListAsync<SCIMRepresentationModel>();
            if (result.Any())
            {
                var references = result.SelectMany(r => r.SchemaRefs).Distinct().ToList();
                var schemas = MongoDBEntity.GetReferences<SCIMSchema>(references, _scimDbContext.Database);
                foreach (var representation in result)
                    representation.Schemas = schemas.Where(s => representation.SchemaRefs.Any(r => r.Id == s.Id)).ToList();
            }

            return result;
        }

        public IEnumerable<IEnumerable<SCIMRepresentation>> FindPaginatedRepresentations(IEnumerable<string> representationIds, string resourceType = null, int nbRecords = 50, bool ignoreAttributes = false)
        {
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var query = _scimDbContext.SCIMRepresentationLst.AsQueryable()
                    .Where(r => filter.Contains(r.Id));
                if (!string.IsNullOrWhiteSpace(resourceType))
                    query = query.Where(r => r.ResourceType == resourceType);
                var result = query.ToMongoListAsync().Result;
                var references = result.SelectMany(r => r.SchemaRefs).Distinct().ToList();
                var schemas = MongoDBEntity.GetReferences<SCIMSchema>(references, _scimDbContext.Database);
                foreach(var representation in result)
                    representation.Schemas = schemas.Where(s => representation.SchemaRefs.Any(r => r.Id == s.Id)).ToList();

                yield return result;
            }
        }

        public IEnumerable<IEnumerable<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var query = _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId);
            var nb = query.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var parentIds = query.Skip(i * nbRecords).Take(nbRecords).ToMongoListAsync().Result;
                var result = _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                    .ToMongoListAsync().Result;
                yield return result;
            }
        }

        public IEnumerable<IEnumerable<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var parentIds = _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                    .Where(a => a.SchemaAttributeId == schemaAttributeId && filter.Contains(a.RepresentationId) && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                    .Select(r => r.ParentAttributeId)
                    .ToMongoListAsync().Result;
                var result = _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                    .ToMongoListAsync().Result;
                yield return result;
            }
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, string value, string endpoint = null)
        {
            var flatAttr = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == value)
                .ToMongoFirstAsync();
            if (flatAttr == null) return null;
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) && r.Id == flatAttr.RepresentationId)
                .ToMongoFirstAsync();
            if (result == null)
                return null;

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            var representationIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueInteger == value)
                .Select(a => a.RepresentationId)
                .ToMongoListAsync();
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) && representationIds.Contains(r.Id))
                .ToMongoFirstAsync();
            if (result == null)
                return null;

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType)
        {
            var representationIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => a.FullPath == fullPath && values.Contains(a.ValueString))
                .Select(a => a.RepresentationId)
                .ToMongoListAsync();
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id))
                .ToMongoListAsync<SCIMRepresentationModel>();
            if (result.Any())
            {
                var references = result.SelectMany(r => r.SchemaRefs).Distinct().ToList();
                var schemas = MongoDBEntity.GetReferences<SCIMSchema>(references, _scimDbContext.Database);
                foreach (var representation in result)
                    representation.Schemas = schemas.Where(s => representation.SchemaRefs.Any(r => r.Id == s.Id)).ToList();
            }

            return result;
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

        public async Task<bool> Add(SCIMRepresentation representation, CancellationToken token)
        {
            var record = new SCIMRepresentationModel(representation, _options.CollectionSchemas, _options.CollectionRepresentationAttributes);
            foreach (var flatAttr in record.FlatAttributes) flatAttr.RepresentationId = representation.Id;
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.InsertOneAsync(_session, record, null, token);
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(_session, record.FlatAttributes, cancellationToken: token);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.InsertOneAsync(record, null, token);
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(record.FlatAttributes, cancellationToken: token);
            }

            return true;
        }

        public async Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            var attributeIds = data.FlatAttributes.Select(a => a.Id);
            var filter = Builders<SCIMRepresentationAttribute>.Filter.In(a => a.Id, attributeIds);
            if(_session != null)
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

        public async Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = new SCIMRepresentationModel(data, _options.CollectionSchemas, _options.CollectionRepresentationAttributes);
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == data.Id, record);
                foreach (var attr in data.FlatAttributes)
                    await _scimDbContext.SCIMRepresentationAttributeLst.ReplaceOneAsync(_session, s => s.Id == attr.Id, attr);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == data.Id, record);
                foreach(var attr in data.FlatAttributes)
                    await _scimDbContext.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr);
            }

            return true;
        }

        public async Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            if (!scimRepresentationAttributes.Any()) return;
            var representationIds = scimRepresentationAttributes.Select(r => r.RepresentationId).Distinct();
            var representations = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(_session, scimRepresentationAttributes);
                foreach (var representation in representations)
                {
                    var attrs = scimRepresentationAttributes.Where(r => r.RepresentationId == representation.Id).ToList();
                    foreach (var attr in attrs) representation.FlatAttributes.Add(attr);
                    await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == representation.Id, representation);
                }
            }
            else
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(scimRepresentationAttributes);
                foreach (var representation in representations)
                {
                    var attrs = scimRepresentationAttributes.Where(r => r.RepresentationId == representation.Id).ToList();
                    foreach (var attr in attrs) representation.FlatAttributes.Add(attr);
                    await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == representation.Id, representation);
                }
            }
        }

        public async Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            if (!scimRepresentationAttributes.Any()) return;
            var representationIds = scimRepresentationAttributes.Select(r => r.RepresentationId).Distinct();
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();
            var attributeIds = scimRepresentationAttributes.Select(a => a.Id);
            var filter = Builders<SCIMRepresentationAttribute>.Filter.In(a => a.Id, attributeIds);
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(_session, filter);
                foreach (var representation in result)
                {
                    var removedAttributeIds = scimRepresentationAttributes.Where(r => r.RepresentationId == representation.Id).Select(r => r.Id);
                    representation.FlatAttributes = representation.FlatAttributes.Where(r => !removedAttributeIds.Contains(r.Id)).ToList();
                    await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == representation.Id, representation);
                }
            }
            else
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(filter);
                foreach (var representation in result)
                {
                    var removedAttributeIds = scimRepresentationAttributes.Where(r => r.RepresentationId == representation.Id).Select(r => r.Id);
                    representation.FlatAttributes = representation.FlatAttributes.Where(r => !removedAttributeIds.Contains(r.Id)).ToList();
                    await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == representation.Id, representation);
                }
            }
        }

        public async Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            if (!scimRepresentationAttributes.Any()) return;
            var representationIds = scimRepresentationAttributes.Select(r => r.RepresentationId).Distinct();
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(r => representationIds.Contains(r.Id)).ToMongoListAsync();
            if (_session != null)
            {
                foreach(var attr in scimRepresentationAttributes)
                    await _scimDbContext.SCIMRepresentationAttributeLst.ReplaceOneAsync(_session, s => s.Id == attr.Id, attr);
                foreach (var representation in result)
                {
                    var updatedAttributes = scimRepresentationAttributes.Where(r => r.RepresentationId == representation.Id);
                    representation.FlatAttributes = representation.FlatAttributes.Where(a => !updatedAttributes.Any(at => at.Id == a.Id)).ToList();
                    foreach (var attr in updatedAttributes)
                        representation.FlatAttributes.Add(attr);
                    await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(_session, s => s.Id == representation.Id, representation);
                }
            }
            else
            {
                foreach (var attr in scimRepresentationAttributes)
                    await _scimDbContext.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr);
                foreach (var representation in result)
                {
                    var updatedAttributes = scimRepresentationAttributes.Where(r => r.RepresentationId == representation.Id);
                    representation.FlatAttributes = representation.FlatAttributes.Where(a => !updatedAttributes.Any(at => at.Id == a.Id)).ToList();
                    foreach (var attr in updatedAttributes)
                        representation.FlatAttributes.Add(attr);
                    await _scimDbContext.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == representation.Id, representation);
                }
            }
        }
    }
}
