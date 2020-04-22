// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
    {
        private readonly IMongoDatabase _database;

        public SCIMRepresentationCommandRepository(IOptions<MongoDbOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.Database);
        }

        public bool Add(SCIMRepresentation data)
        {            
            var representations = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            representations.InsertOne(data);
            return true;
        }

        public bool Delete(SCIMRepresentation data)
        {
            var representations = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            var deleteFilter = Builders<SCIMRepresentation>.Filter.Eq("_id", data.Id);
            representations.DeleteOne(deleteFilter);
            return true;
        }

        public bool Update(SCIMRepresentation data)
        {
            Delete(data);
            Add(data);
            return true;
        }

        public Task<int> SaveChanges()
        {
            return Task.FromResult(1);
            // return _scimDbContext.SaveChangesAsync();
        }

        public void Dispose() { }
    }
}
