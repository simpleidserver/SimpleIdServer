// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
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

            var newAttributes = representation.Attributes.Select(a => new SCIMRepresentationAttributeModel(a, representation.Id, representation.ResourceType));
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(_session, newAttributes, null, token);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.InsertManyAsync(newAttributes, null, token);
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

            var filter = Builders<SCIMRepresentationAttributeModel>.Filter.Eq(s => s.RepresentationId, data.Id);
            if (_session != null)
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(_session, filter, null, token);
            }
            else
            {
                await _scimDbContext.SCIMRepresentationAttributeLst.DeleteManyAsync(filter, null, token);
            }

            return true;
        }

        public async Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            await Delete(data, token);
            await Add(data, token);
            return true;
        }
    }
}
