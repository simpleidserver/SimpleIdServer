// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMRepresentationAttributeModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string SchemaAttributeId { get; set; }
        public string RepresentationId { get; set; }
        public virtual IQueryable<string> ValuesString { get; set; }
        public virtual IQueryable<bool> ValuesBoolean { get; set; }
        public virtual IQueryable<int> ValuesInteger { get; set; }
        public virtual IQueryable<DateTime> ValuesDateTime { get; set; }
        public virtual IQueryable<string> ValuesReference { get; set; }
        public virtual ICollection<SCIMRepresentationAttributeModel> Values { get; set; }
        public virtual SCIMRepresentationAttributeModel Parent { get; set; }
        public virtual SCIMSchemaAttributeModel SchemaAttribute { get; set; }
        public virtual SCIMRepresentationModel Representation { get; set; }
    }
}
