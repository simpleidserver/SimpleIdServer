// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
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

        public SCIMRepresentationCommandRepository(SCIMDbContext scimDbContext, IMongoClient mongoClient, IOptions<MongoDbOptions> options)
        {
            _scimDbContext = scimDbContext;
            _mongoClient = mongoClient;
            _options = options.Value;
        }

        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            var session = await _mongoClient.StartSessionAsync(null, token);
            session.StartTransaction();
            return new MongoDbTransaction(session);
        }

        public async Task<bool> Add(SCIMRepresentation data, CancellationToken token)
        {
            var representations = _scimDbContext.SCIMRepresentationLst;
            var record = new SCIMRepresentationModel
            {
                Created = data.Created,
                ExternalId = data.ExternalId,
                Id = data.Id,
                LastModified = data.LastModified,
                ResourceType = data.ResourceType,
                Version = data.Version,
                Attributes = data.Attributes.Select(_ => _.ToModel()).ToList()
            };
            record.SetSchemas(data.Schemas.Select(_ => _.ToModel()).ToList(), _options.CollectionSchemas);
            await representations.InsertOneAsync(record, null, token);
            return true;
        }

        public async Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            var representations = _scimDbContext.SCIMRepresentationLst;
            var deleteFilter = Builders<SCIMRepresentationModel>.Filter.Eq("_id", data.Id);
            await representations.DeleteOneAsync(deleteFilter, null, token);
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
