// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
        public virtual ICollection<SCIMRepresentationAttributeValueModel> Values { get; set; }
        public virtual ICollection<SCIMRepresentationAttributeModel> Children { get; set; }
        public virtual SCIMRepresentationAttributeModel Parent { get; set; }
        public virtual SCIMSchemaAttributeModel SchemaAttribute { get; set; }
        public virtual SCIMRepresentationModel Representation { get; set; }
    }
}
