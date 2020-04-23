// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMRepresentationAttributeModel
    {
        public SCIMRepresentationAttributeModel()
        {
            Children = new List<SCIMRepresentationAttributeModel>();
        }

        public string Id { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public ICollection<bool> ValuesBoolean { get; set; }
        public ICollection<int> ValuesInteger { get; set; }
        public ICollection<DateTime> ValuesDateTime { get; set; }
        public ICollection<string> ValuesReference { get; set; }
        public ICollection<SCIMRepresentationAttributeModel> Children { get; set; }
        public MongoDBRef Parent { get; set; }
        public SCIMSchemaAttributeModel SchemaAttribute { get; set; }
    }
}
