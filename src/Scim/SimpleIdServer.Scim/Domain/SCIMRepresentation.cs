// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
        public string ResourceType { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public virtual ICollection<SCIMRepresentationAttribute> Attributes { get; set; }
        public virtual ICollection<SCIMSchema> Schemas { get; set; }

        public void AddAttribute(SCIMRepresentationAttribute attribute)
        {
            Attributes.Add(attribute);
        }

        public void SetCreated(DateTime created)
        {
            Created = created;
            var meta = TryAddMetaAttribute();
            meta.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), meta.SchemaAttribute.SubAttributes.First(a => a.Name == SCIMConstants.StandardSCIMMetaAttributes.Created), valuesDateTime: new List<DateTime> { created }));
        }

        public void SetUpdated(DateTime lastModified)
        {
            LastModified = lastModified;
            var meta = TryAddMetaAttribute();
            meta.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), meta.SchemaAttribute.SubAttributes.First(a => a.Name == SCIMConstants.StandardSCIMMetaAttributes.LastModified), valuesDateTime: new List<DateTime> { lastModified }));
        }

        public void SetVersion(string version)
        {
            Version = version;
            var meta = TryAddMetaAttribute();
            meta.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), meta.SchemaAttribute.SubAttributes.First(a => a.Name == SCIMConstants.StandardSCIMMetaAttributes.Version), valuesString: new List<string> { version }));
        }

        public void SetResourceType(string resourceType)
        {
            ResourceType = resourceType;
            var meta = TryAddMetaAttribute();
            meta.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), meta.SchemaAttribute.SubAttributes.First(a => a.Name == SCIMConstants.StandardSCIMMetaAttributes.ResourceType), valuesString: new List<string> { resourceType }));
        }

        public object Clone()
        {
            return new SCIMRepresentation
            {
                Id = Id,
                ResourceType = ResourceType,
                Version = Version,
                Created = Created,
                LastModified = LastModified,
                Attributes = Attributes.Select(a => (SCIMRepresentationAttribute)a.Clone()).ToList(),
                Schemas = Schemas.Select(a => (SCIMSchema)a.Clone()).ToList()
            };
        }

        private SCIMRepresentationAttribute TryAddMetaAttribute()
        {
            var metaAttribute = Attributes.FirstOrDefault(a => a.SchemaAttribute.Name == SCIMConstants.StandardSCIMRepresentationAttributes.Meta);
            if (metaAttribute == null)
            {
                var metaSchemaAttribute = SCIMConstants.StandardSchemas.CommonSchema.Attributes.First();
                metaAttribute = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), metaSchemaAttribute);
                Attributes.Add(metaAttribute);
            }

            return metaAttribute;
        }
    }
}
