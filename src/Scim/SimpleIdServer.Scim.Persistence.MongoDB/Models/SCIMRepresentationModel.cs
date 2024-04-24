// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Infrastructures;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMRepresentationModel : SCIMRepresentation
    {
        public SCIMRepresentationModel() : base()
        {
            SchemaRefs = new List<CustomMongoDBRef>();
            AttributeRefs = new List<CustomMongoDBRef>();
        }

        public SCIMRepresentationModel(SCIMRepresentation representation, string schemaCollectionName, string attributesCollectionName) : this()
        {
            Id = representation.Id;
            RealmName = representation.RealmName;
            ExternalId = representation.ExternalId;
            ResourceType = representation.ResourceType;
            Version = representation.Version;
            Created = representation.Created;
            LastModified = representation.LastModified;
            DisplayName = representation.DisplayName;
            AttributeRefs = representation.FlatAttributes.Select(s => new CustomMongoDBRef(attributesCollectionName, s.Id)).ToList();
            SchemaRefs = representation.Schemas.Select(s => new CustomMongoDBRef(schemaCollectionName, s.Id)).ToList();
        }

        public ICollection<CustomMongoDBRef> SchemaRefs { get; set; }
        public ICollection<CustomMongoDBRef> AttributeRefs { get; set; }

        public void IncludeAll(IMongoDatabase database)
        {
            IncludeSchemas(database);
            IncludeAttributes(database);
        }

        public void IncludeSchemas(IMongoDatabase database)
        {
            Schemas = MongoDBEntity.GetReferences<SCIMSchema>(SchemaRefs, database);
        }

        public void IncludeAttributes(IMongoDatabase database)
        {
            FlatAttributes = MongoDBEntity.GetReferences<SCIMRepresentationAttribute>(AttributeRefs, database);
        }
    }
}
