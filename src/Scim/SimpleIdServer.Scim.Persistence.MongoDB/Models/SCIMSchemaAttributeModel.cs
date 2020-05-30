// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMSchemaAttributeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [BsonRepresentation(BsonType.String)]
        public SCIMSchemaAttributeTypes Type { get; set; }
        public bool MultiValued { get; set; }
        public ICollection<SCIMSchemaAttributeModel> SubAttributes { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public ICollection<string> CanonicalValues { get; set; }
        public bool CaseExact { get; set; }
        [BsonRepresentation(BsonType.String)]
        public SCIMSchemaAttributeMutabilities Mutability { get; set; }
        [BsonRepresentation(BsonType.String)]
        public SCIMSchemaAttributeReturned Returned { get; set; }
        [BsonRepresentation(BsonType.String)]
        public SCIMSchemaAttributeUniqueness Uniqueness { get; set; }
        public ICollection<string> ReferenceTypes { get; set; }
        public ICollection<string> DefaultValueString { get; set; }
        public ICollection<int> DefaultValueInt { get; set; }
    }
}
