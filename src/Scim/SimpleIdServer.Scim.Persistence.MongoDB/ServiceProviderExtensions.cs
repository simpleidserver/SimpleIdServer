// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB;

public static class ServiceProviderExtensions
{
    public static void EnsureMongoStoreDataMigrated(this IServiceProvider serviceProvider, List<SCIMSchema> initialSchemas, List<SCIMAttributeMapping> initialAttributeMapping, List<Realm> realms)
    {
        var dbcontext = serviceProvider.GetService<SCIMDbContext>();
        var options = dbcontext.Options;
        EnsureCollectionIsCreated<SCIMRepresentationModel>(dbcontext.Database, options.CollectionRepresentations);
        EnsureCollectionIsCreated<SCIMRepresentationAttribute>(dbcontext.Database, options.CollectionRepresentationAttributes);
        EnsureCollectionIsCreated<ProvisioningConfiguration>(dbcontext.Database, options.CollectionProvisioningLst);
        EnsureSCIMRepresentationAttributeIndexesAreCreated(dbcontext.Database, options.CollectionRepresentationAttributes);
        var schemasCollection = EnsureCollectionIsCreated<SCIMSchema>(dbcontext.Database, options.CollectionSchemas);
        var mappingsCollection = EnsureCollectionIsCreated<SCIMAttributeMapping>(dbcontext.Database, options.CollectionMappings);
        var realmsCollection = EnsureCollectionIsCreated<Realm>(dbcontext.Database, options.CollectionRealms);
        var query = schemasCollection.AsQueryable();
        if (query.Count() == 0)
        {
            if (initialSchemas != null)
            {
                schemasCollection.InsertMany(initialSchemas);
            }
            else
            {
                var schemas = new List<SCIMSchema>
                {
                    StandardSchemas.GroupSchema,
                    StandardSchemas.UserSchema
                };
                schemasCollection.InsertMany(schemas);
            }
        }

        if (mappingsCollection.AsQueryable().Count() == 0)
        {
            if (initialAttributeMapping != null)
            {
                mappingsCollection.InsertMany(initialAttributeMapping);
            }
            else
            {
                mappingsCollection.InsertMany(SCIMConstants.StandardAttributeMapping);
            }
        }

        if (realmsCollection.AsQueryable().Count() == 0)
        {
            if (realms != null)
            {
                realmsCollection.InsertMany(realms);
            }
            else
            {
                realmsCollection.InsertMany(SCIMConstants.StandardRealms);
            }
        }
    }

    private static void EnsureSCIMRepresentationAttributeIndexesAreCreated(IMongoDatabase db, string name)
    {
        var caseInsensitiveCollation = new Collation("en", strength: CollationStrength.Secondary);
        var valueStringCaseInsensitiveOptions = new CreateIndexOptions { Collation = caseInsensitiveCollation };
        var compoundIndex = Builders<SCIMRepresentationAttribute>.IndexKeys.Ascending(a => a.RepresentationId).Ascending(a => a.SchemaAttributeId).Ascending(a => a.ValueString);
        var representationIdIndex = Builders<SCIMRepresentationAttribute>.IndexKeys.Ascending(a => a.RepresentationId);
        var valueStringIndex = Builders<SCIMRepresentationAttribute>.IndexKeys.Ascending(a => a.ValueString);
        EnsureIndexCreated(db, "RepresentationId_1_SchemaAttributeId_1_ValueString_1", name, compoundIndex);
        EnsureIndexCreated(db, "RepresentationId_1", name, representationIdIndex);
        EnsureIndexCreated(db, "ValueString_1_case_insensitive", name, valueStringIndex, valueStringCaseInsensitiveOptions);
    }

    private static async void EnsureIndexCreated(IMongoDatabase db, string indexName, string name, IndexKeysDefinition<SCIMRepresentationAttribute> indexDefinition, CreateIndexOptions options = null)
    {
        var collection = db.GetCollection<SCIMRepresentationAttribute>(name);
        var indexes = await collection.Indexes.List().ToListAsync();
        if (indexes.Any(i => i.Elements.Any(e => e.Name == "name" && e.Value.AsString == indexName))) return;
        if (options != null)
        {
            var indexModel = new CreateIndexModel<SCIMRepresentationAttribute>(indexDefinition, options);
            collection.Indexes.CreateOne(indexModel);
        }
        else
        {
            collection.Indexes.CreateOne(indexDefinition);
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
