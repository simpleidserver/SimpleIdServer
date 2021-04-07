// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMRepresentation : ICloneable, IEquatable<SCIMRepresentation>
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
        public string ExternalId { get; set; }
        public string ResourceType { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<SCIMRepresentationAttribute> Attributes { get; set; }
        public ICollection<SCIMSchema> Schemas { get; set; }

        public void AddAttribute(SCIMRepresentationAttribute attribute)
        {
            Attributes.Add(attribute);
        }

        public void RemoveAttributes(IEnumerable<string> schemaAttrIds)
        {
            Attributes = Attributes.Where(_ => !schemaAttrIds.Contains(_.SchemaAttribute.Id)).ToList();
        }

        public SCIMRepresentationAttribute GetAttribute(string fullPath)
        {
            var splitted = fullPath.Split('.').ToList();
            return Attributes.GetAttribute(splitted);
        }

        public ICollection<SCIMRepresentationAttribute> GetAttributesByAttrSchemaId(string attrSchemaId)
        {
            var result = new List<SCIMRepresentationAttribute>();
            Attributes.GetAttributesByAttrSchemaId(attrSchemaId, result);
            return result;
        }

        public SCIMRepresentationAttribute GetParentAttribute(string fullPath)
        {
            var splitted = fullPath.Split('.').ToList();
            if (splitted.Count <= 1)
            {
                return null;
            }

            splitted.Remove(splitted.Last());
            return Attributes.GetAttribute(splitted);
        }

        public void SetCreated(DateTime created)
        {
            Created = created;
        }

        public void SetExternalId(string externalId)
        {
            ExternalId = externalId;
        }

        public void SetUpdated(DateTime lastModified)
        {
            LastModified = lastModified;
        }

        public void SetVersion(string version)
        {
            Version = version;
        }

        public void SetResourceType(string resourceType)
        {
            ResourceType = resourceType;
        }

        public bool ContainsAttribute(SCIMRepresentationAttribute attr)
        {
            return Attributes.Any(a => a.IsSimilar(attr));
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

        public bool Equals(SCIMRepresentation other)
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
