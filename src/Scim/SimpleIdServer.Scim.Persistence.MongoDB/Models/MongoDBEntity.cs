// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public static class MongoDBEntity
    {
        public static ICollection<T> GetReferences<T>(ICollection<MongoDBRef> refs, IMongoDatabase mongoDatabase) where T : BaseDomain
        {
            if(!refs.Any())
            {
                return new List<T>();
            }

            var collectionName = refs.First().CollectionName;
            var ids = refs.Select(r => r.Id);
            var filter = Builders<T>.Filter.In(x => x.Id, ids);
            return mongoDatabase.GetCollection<T>(collectionName).Find(filter).ToList();
        }
    }
}
