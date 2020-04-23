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
        private readonly SCIMDbContext _scimDbContext;

        public SCIMRepresentationCommandRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public bool Add(SCIMRepresentation data)
        {            
            var representations = _scimDbContext.SCIMRepresentationLst;
            representations.InsertOne(data);
            return true;
        }

        public bool Delete(SCIMRepresentation data)
        {
            var representations = _scimDbContext.SCIMRepresentationLst;
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
