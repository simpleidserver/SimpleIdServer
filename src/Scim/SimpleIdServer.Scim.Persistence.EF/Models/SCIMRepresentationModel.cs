// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMRepresentationModel : IEquatable<SCIMRepresentationModel>
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string ResourceType { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public virtual ICollection<SCIMRepresentationAttributeModel> Attributes { get; set; }
        public virtual ICollection<SCIMRepresentationSchemaModel> Schemas { get; set; }

        public bool Equals(SCIMRepresentationModel other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
