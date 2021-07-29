// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMSchema : ICloneable
    {
        public SCIMSchema()
        {
            Attributes = new List<SCIMSchemaAttribute>();
            SchemaExtensions = new List<SCIMSchemaExtension>();
        }

        /// <summary>
        /// The unique URI of the schema. When applicable, service providers MUST specify the URI, e.g., "urn:ietf:params:scim:schemas:core:2.0:User".
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The schema's human-readable name. 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The schema's human-readable description. 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is root schema.
        /// </summary>
        public bool IsRootSchema { get; set; }
        /// <summary>
        /// Resource type.
        /// </summary>
        public string ResourceType { get; set; }
        /// <summary>
        /// Gets or sets the schema extensions.
        /// </summary>
        public ICollection<SCIMSchemaExtension> SchemaExtensions { get; set; }
        /// <summary>
        /// A complex type that defines service provider attributes and their qualities.
        /// </summary>
        public ICollection<SCIMSchemaAttribute> Attributes { get; set; }
        public ICollection<SCIMRepresentation> Representations { get; set; }

        public List<TreeNode<SCIMSchemaAttribute>> HierarchicalAttributes
        {
            get
            {
                return BuildHierarchicalAttributes(null);
            }
        }

        public SCIMSchemaAttribute GetAttribute(string path)
        {
            return Attributes.FirstOrDefault(a => a.FullPath == path);
        }

        public SCIMSchemaAttribute GetAttributeById(string attributeId)
        {
            return Attributes.FirstOrDefault(a => a.Id == attributeId);
        }

        public IEnumerable<SCIMSchemaAttribute> GetChildren(SCIMSchemaAttribute schemaAttribute)
        {
            return GetChildren(schemaAttribute.Id);
        }

        public IEnumerable<SCIMSchemaAttribute> GetChildren(string id)
        {
            return Attributes.Where(a => a.ParentId == id);
        }

        public void AddAttribute(SCIMSchemaAttribute parentAttr, SCIMSchemaAttribute childAttr)
        {
            childAttr.ParentId = parentAttr.Id;
            childAttr.FullPath = $"{parentAttr.FullPath}.{childAttr.Name}";
            Attributes.Add(childAttr);
        }

        public bool HasAttribute(SCIMSchemaAttribute attribute)
        {
            return Attributes.Any(attr => attr.Id == attribute.Id);
        }

        public object Clone()
        {
            return new SCIMSchema
            {
                Id = Id,
                Name = Name,
                Description = Description,
                IsRootSchema = IsRootSchema,
                Attributes = Attributes.Select(a => (SCIMSchemaAttribute)a.Clone()).ToList(),
                SchemaExtensions = SchemaExtensions.Select(a => (SCIMSchemaExtension)a.Clone()).ToList()
            };
        }

        private List<TreeNode<SCIMSchemaAttribute>> BuildHierarchicalAttributes(string parentId)
        {
            var nodes = Attributes.Where(p => (string.IsNullOrWhiteSpace(parentId) && string.IsNullOrWhiteSpace(p.ParentId)) || p.ParentId == parentId).Select(s =>
            {
                return new TreeNode<SCIMSchemaAttribute>(s);
            }).ToList();
            foreach(var node in nodes)
            {
                node.AddChildren(BuildHierarchicalAttributes(node.Leaf.Id));
            }

            return nodes;
        }
    }
}
