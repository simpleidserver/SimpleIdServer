// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMRepresentationModel : SCIMRepresentation
    {
        public SCIMRepresentationModel() : base()
        {
            SchemaRefs = new List<MongoDBRef>();
        }

        public SCIMRepresentationModel(SCIMRepresentation representation, string schemaCollectionName, string attributesCollectionName) : this()
        {
            Id = representation.Id;
            ExternalId = representation.ExternalId;
            ResourceType = representation.ResourceType;
            Version = representation.Version;
            Created = representation.Created;
            LastModified = representation.LastModified;
            DisplayName = representation.DisplayName;
            FlatAttributes = representation.FlatAttributes;
            SchemaRefs = representation.Schemas.Select(s => new MongoDBRef(schemaCollectionName, s.Id)).ToList();
        }

        public ICollection<MongoDBRef> SchemaRefs { get; set; }

        public void IncludeAll(IMongoDatabase database)
        {
            IncludeSchemas(database);
        }

        public void IncludeSchemas(IMongoDatabase database)
        {
            Schemas = MongoDBEntity.GetReferences<SCIMSchema>(SchemaRefs, database);
        }
    }
}
