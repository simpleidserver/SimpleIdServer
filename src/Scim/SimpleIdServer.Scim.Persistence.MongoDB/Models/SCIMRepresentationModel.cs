// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMRepresentationModel
    {
        public SCIMRepresentationModel()
        {
            SchemaRefs = new List<MongoDBRef>();
            Attributes = new List<SCIMRepresentationAttributeModel>();
        }

        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string ResourceType { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<SCIMRepresentationAttributeModel> Attributes { get; set; }
        
        public void SetSchemas(List<SCIMSchemaModel> schemas, string collectionName)
        {
            SchemaRefs = schemas.Select(_ => new MongoDBRef(collectionName, _.Id)).ToList();
        }

        public ICollection<SCIMSchemaModel> GetSchemas(IMongoDatabase db)
        {
            var result = new List<SCIMSchemaModel>();
            if (!SchemaRefs.Any())
            {
                return result;
            }

            var collectionName = SchemaRefs.First().CollectionName;
            var schemaIds = SchemaRefs.Select(_ => _.Id).ToList();
            var filter = Builders<SCIMSchemaModel>.Filter.In(x => x.Id, schemaIds);
            return db.GetCollection<SCIMSchemaModel>(collectionName).Find(filter).ToList();
        }

        public ICollection<MongoDBRef> SchemaRefs { get; set; } 
    }
}
