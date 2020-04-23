// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public static class MongoDbClientExtensions
	{
		public static void EnsureMongoDbSCIMDatabaseIsCreated(
			MongoDbOptions options,
			List<SCIMSchema> initialSchemas,
			List<SCIMAttributeMapping> initialAttributeMapping
			)
		{
			var mongoClient = new MongoClient(options.ConnectionString);
			var db = mongoClient.GetDatabase(options.Database);
			var schemasCollection = EnsureCollectionIsCreated<SCIMSchemaModel>(db, options.CollectionSchemas);
			var mappingsCollection = EnsureCollectionIsCreated<SCIMAttributeMappingModel>(db, options.CollectionMappings);
			EnsureCollectionIsCreated<SCIMRepresentationModel>(db, options.CollectionRepresentations);
			var query = schemasCollection.AsQueryable();
			if (query.Count() == 0)
			{
				if (initialSchemas != null)
				{
					schemasCollection.InsertMany(initialSchemas.Select(_ => _.ToModel()));
				}
				else
				{
					var schemas = new List<SCIMSchemaModel>
					{
						SCIMConstants.StandardSchemas.GroupSchema.ToModel(),
						SCIMConstants.StandardSchemas.UserSchema.ToModel()
                    };
					schemasCollection.InsertMany(schemas);
				}
			}

			if (mappingsCollection.AsQueryable().Count() == 0)
			{
				if (initialAttributeMapping != null)
					mappingsCollection.InsertMany(initialAttributeMapping.Select(_ => _.ToModel()));
				else
					mappingsCollection.InsertMany(SCIMConstants.StandardAttributeMapping.Select(_ => _.ToModel()));
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
