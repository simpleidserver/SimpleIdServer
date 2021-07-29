// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
        public int VersionNumber { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<SCIMRepresentationAttribute> Attributes { get; set; }
        public ICollection<SCIMSchema> Schemas { get; set; }
        public List<TreeNode<SCIMRepresentationAttribute>> HierarchicalAttributes
        {
            get
            {
                return BuildHierarchicalAttributes(null);
            }
        }

        public SCIMSchema GetRootSchema()
        {
            return Schemas.FirstOrDefault(s => s.IsRootSchema);
        }

        public IEnumerable<SCIMSchema> GetExtensionSchemas()
        {
            return Schemas.Where(s => !s.IsRootSchema);
        }

        public SCIMSchema GetSchema(SCIMRepresentationAttribute attribute)
        {
            return GetSchema(attribute.SchemaAttribute);
        }

        public SCIMSchema GetSchema(SCIMSchemaAttribute attribute)
        {
            return Schemas.FirstOrDefault(s => s.HasAttribute(attribute));
        }

        public SCIMSchema GetSchema(string id)
        {
            return Schemas.FirstOrDefault(s => s.Id == id);
        }

        public void AddAttribute(SCIMRepresentationAttribute attribute)
        {
            Attributes.Add(attribute);
        }
        
        public void AddAttribute(SCIMRepresentationAttribute parentAttribute, SCIMRepresentationAttribute childAttribute)
        {
            childAttribute.ParentAttributeId = parentAttribute.Id;
            Attributes.Add(childAttribute);
        }

        public void RemoveAttribute(SCIMRepresentationAttribute attribute)
        {
            Attributes.Remove(attribute);
        }

        public void RemoveAttributes(IEnumerable<string> schemaAttrIds)
        {
            Attributes = Attributes.Where(_ => !schemaAttrIds.Contains(_.SchemaAttribute.Id)).ToList();
        }

        public SCIMRepresentationAttribute GetAttribute(string fullPath)
        {
            return Attributes.FirstOrDefault(a => a.FullPath == fullPath);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetAttributesByAttrSchemaId(string attrSchemaId)
        {
            return Attributes.Where(a => a.SchemaAttributeId == attrSchemaId);
        }

        public SCIMRepresentationAttribute GetParentAttribute(SCIMRepresentationAttribute attr)
        {
            return GetParentAttribute(attr.FullPath);
        }

        public SCIMRepresentationAttribute GetParentAttribute(string fullPath)
        {
            var splitted = fullPath.Split('.').ToList();
            if (splitted.Count <= 1)
            {
                return null;
            }

            splitted.Remove(splitted.Last());
            fullPath = string.Join(".", splitted);
            return GetAttribute(fullPath);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetChildren(SCIMRepresentationAttribute attr)
        {
            return Attributes.Where(a => a.ParentAttributeId == attr.Id);
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
            VersionNumber++;
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
            return false;
            // return Attributes.Any(a => a.IsSimilar(attr));
        }

        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public object Clone()
        {
            return new SCIMRepresentation
            {
                Id = Id,
                ExternalId = ExternalId,
                ResourceType = ResourceType,
                Version = Version,
                VersionNumber = VersionNumber,
                Created = Created,
                LastModified = LastModified,
                DisplayName = DisplayName,
                Attributes = Attributes.Select(a => (SCIMRepresentationAttribute)a.Clone()).ToList(),
                Schemas = Schemas.Select(a => (SCIMSchema)a.Clone()).ToList()
            };
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var target = obj as SCIMRepresentation;
            if (target == null)
            {
                return false;
            }

            return this.Equals(target);
        }

        public bool Equals(SCIMRepresentation other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private List<TreeNode<SCIMRepresentationAttribute>> BuildHierarchicalAttributes(string parentId)
        {
            var nodes = Attributes.Where(p => (string.IsNullOrWhiteSpace(parentId) && string.IsNullOrWhiteSpace(p.ParentAttributeId)) || p.ParentAttributeId == parentId).Select(s =>
            {
                return new TreeNode<SCIMRepresentationAttribute>(s);
            }).ToList();
            foreach (var node in nodes)
            {
                node.AddChildren(BuildHierarchicalAttributes(node.Leaf.Id));
            }

            return nodes;
        }
    }
}
