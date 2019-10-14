using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMRepresentation : ICloneable
    {
        public SCIMRepresentation()
        {
            Schemas = new List<SCIMSchema>();
            Attributes = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentation(ICollection<SCIMSchema> schemas, ICollection<SCIMRepresentationAttribute> attributes)
        {
            Schemas = schemas;
            Attributes = attributes;
        }

        public string Id { get; set; }
        /// <summary>
        /// External identifier.
        /// </summary>
        public string ExternalId { get; set; }
        public string ResourceType { get; set; }
        /// <summary>
        /// The version of the resource.
        /// </summary>
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public virtual ICollection<SCIMRepresentationAttribute> Attributes { get; set; }
        public virtual ICollection<SCIMSchema> Schemas { get; set; }

        public void AddAttribute(SCIMRepresentationAttribute attribute)
        {
            Attributes.Add(attribute);
        }

        public object Clone()
        {
            return new SCIMRepresentation
            {
                Id = Id,
                ExternalId = ExternalId,
                ResourceType = ResourceType,
                Version = Version,
                Created = Created,
                LastModified = LastModified,
                Attributes = Attributes.Select(a => (SCIMRepresentationAttribute)a.Clone()).ToList(),
                Schemas = Schemas.Select(a => (SCIMSchema)a.Clone()).ToList()
            };
        }
    }
}
