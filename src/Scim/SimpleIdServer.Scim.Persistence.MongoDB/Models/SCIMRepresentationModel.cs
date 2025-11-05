// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Infrastructures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMRepresentationModel : SCIMRepresentation
    {
        public SCIMRepresentationModel() : base()
        {
            SchemaRefs = new List<CustomMongoDBRef>();
        }

        public SCIMRepresentationModel(SCIMRepresentation representation, string schemaCollectionName) : this()
        {
            Id = representation.Id;
            RealmName = representation.RealmName;
            ExternalId = representation.ExternalId;
            ResourceType = representation.ResourceType;
            Version = representation.Version;
            Created = representation.Created;
            LastModified = representation.LastModified;
            DisplayName = representation.DisplayName;
            SchemaRefs = representation.Schemas.Select(s => new CustomMongoDBRef(schemaCollectionName, s.Id)).ToList();
        }

        public ICollection<CustomMongoDBRef> SchemaRefs { get; set; }
        public ICollection<CustomMongoDBRef> AttributeRefs { get; set; }

        public async Task IncludeAll(SCIMDbContext dbContext)
        {
            IncludeSchemas(dbContext.Database);
            await IncludeAttributes(dbContext);
        }

        public void IncludeSchemas(IMongoDatabase database)
        {
            Schemas = MongoDBEntity.GetReferences<SCIMSchema>(SchemaRefs, database);
        }

        public async Task IncludeAttributes(SCIMDbContext dbContext)
        {
            // Optimize MongoDB query for large result sets by using Find with explicit options
            // This provides better performance than LINQ for large collections (e.g., groups with 20k+ members)
            var filter = Builders<SCIMRepresentationAttribute>.Filter.Eq(a => a.RepresentationId, Id);
            var findOptions = new FindOptions<SCIMRepresentationAttribute>
            {
                // Use configured batch size to reduce round trips to MongoDB
                BatchSize = dbContext.Options.BatchSize
            };

            var cursor = await dbContext.SCIMRepresentationAttributeLst
                .FindAsync(filter, findOptions);
            
            FlatAttributes = await cursor.ToListAsync();
        }
    }
}
