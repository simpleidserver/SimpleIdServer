// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
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

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id))
                .ToMongoListAsync<SCIMRepresentationModel>();
            if (result.Any())
            {
                var references = result.SelectMany(r => r.SchemaRefs).Distinct().ToList();
                var schemas = MongoDBEntity.GetReferences<SCIMSchema>(references, _scimDbContext.Database);
                foreach (var representation in result)
                {
                    representation.Schemas = schemas.Where(s => representation.SchemaRefs.Any(r => r.Id == s.Id)).ToList();
                }
            }

            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, string value, string endpoint = null)
        {
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) && r.FlatAttributes.Any(a => a.SchemaAttribute.Id == schemaAttributeId && a.ValueString == value))
                .ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.Init(_scimDbContext.Database);
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) && r.FlatAttributes.Any(a => a.SchemaAttribute.Id == schemaAttributeId && a.ValueInteger == value))
                .ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.Init(_scimDbContext.Database);
            return result;
        }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> value, string resourceType)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SCIMRepresentation> Get(string representationId, CancellationToken token)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.Init(_scimDbContext.Database);
            return result;
        }

        public async Task<bool> Add(SCIMRepresentation representation, CancellationToken token)
        {
            var record = new SCIMRepresentationModel(representation, _options.CollectionSchemas);
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.InsertOneAsync(_session, record, null, token);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.InsertOneAsync(record, null, token);
            }

            return true;
        }

        public async Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            if(_session != null)
            {
                await _scimDbContext.SCIMRepresentationLst.DeleteOneAsync(_session, d => d.Id == data.Id, null, token);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationLst.DeleteOneAsync(d => d.Id == data.Id, null, token);
            }

            return true;
        }

        public async Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = new SCIMRepresentationModel(data, _options.CollectionSchemas);
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
    }
}
