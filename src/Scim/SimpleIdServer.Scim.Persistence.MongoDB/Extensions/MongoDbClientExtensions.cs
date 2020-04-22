// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public static class MongoDbClientExtensions
    {
        public static void EnsureMongoDbSCIMDatabaseIsCreated(this MongoClient mongoClient, string databaseName)
        {
            var db = mongoClient.GetDatabase(databaseName);
            var schemasCollection = EnsureCollectionIsCreated<SCIMSchema>(db, Constants.CollectionNames.Schemas);
            var mappingsCollection = EnsureCollectionIsCreated<SCIMAttributeMapping>(db, Constants.CollectionNames.Mappings);
            EnsureCollectionIsCreated<SCIMRepresentation>(db, Constants.CollectionNames.Representations);
            var query = schemasCollection.AsQueryable();
            if (query.Count() == 0)
            {
                var schemas = new List<SCIMSchema>
                {
                    SCIMConstants.StandardSchemas.GroupSchema,
                    SCIMConstants.StandardSchemas.UserSchema
                };
                schemasCollection.InsertMany(schemas);
            }

            if (mappingsCollection.AsQueryable().Count() == 0)
            {
                mappingsCollection.InsertOne(SCIMConstants.StandardAttributeMapping.First());
            }
        }

        private static IMongoCollection<T> EnsureCollectionIsCreated<T>(IMongoDatabase db, string name)
        {
            if (!CollectionExists(db, name))
            {
                db.CreateCollection(name);
            }

            return db.GetCollection<T>(name);
        }

        private static bool CollectionExists(IMongoDatabase db, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = db.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }
    }
}
