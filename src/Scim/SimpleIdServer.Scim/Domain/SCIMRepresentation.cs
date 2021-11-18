// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMRepresentation : BaseDomain, ICloneable, IEquatable<SCIMRepresentation>
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

        public string ExternalId { get; set; }
        public string ResourceType { get; set; }
        public int Version { get; set; }
        public string DisplayName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<SCIMRepresentationAttribute> Attributes { get; set; }
        public ICollection<SCIMSchema> Schemas { get; set; }
        public List<TreeNode<SCIMRepresentationAttribute>> HierarchicalAttributes
        {
            get
            {
                return BuildHierarchicalAttributes();
            }
        }

        public SCIMSchemaAttribute GetSchemaAttributeById(string id)
        {
            foreach (var schema in Schemas)
            {
                var attr = schema.GetAttributeById(id);
                if (attr != null)
                {
                    return attr;
                }
            }

            return null;
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

        public void UpdateAttribute(SCIMRepresentationAttribute attribute)
        {
            var attr = Attributes.First(a => a.Id == attribute.Id);
            attr.ValueBinary = attribute.ValueBinary;
            attr.ValueBoolean = attribute.ValueBoolean;
            attr.ValueDateTime = attribute.ValueDateTime;
            attr.ValueDecimal = attribute.ValueDecimal;
            attr.ValueInteger = attribute.ValueInteger;
            attr.ValueReference = attribute.ValueReference;
            attr.ValueString = attribute.ValueString;
        }

        public void AddAttribute(SCIMRepresentationAttribute parentAttribute, SCIMRepresentationAttribute childAttribute)
        {
            childAttribute.ParentAttributeId = parentAttribute.Id;
            Attributes.Add(childAttribute);
        }

        public ICollection<SCIMRepresentationAttribute> RemoveAttributeById(SCIMRepresentationAttribute attribute)
        {
            return RemoveAttributesById(new[] { attribute.Id });
        }

        public void RemoveAttributesBySchemaAttrId(IEnumerable<string> schemaAttrIds)
        {
            for (int i = 0; i < schemaAttrIds.Count(); i++)
            {
                var schemaAttrId = schemaAttrIds.ElementAt(i);
                var attrs = GetAttributesByAttrSchemaId(schemaAttrId).Select(a => a.Id);
                RemoveAttributesById(attrs);
            }
        }

        public ICollection<SCIMRepresentationAttribute> RemoveAttributesById(IEnumerable<string> attrIds)
        {
            var result = new List<SCIMRepresentationAttribute>();
            RemoveAttributesById(result, attrIds);
            return result;
        }

        public void RemoveAttributesById(List<SCIMRepresentationAttribute> removedAttrs, IEnumerable<string> attrIds)
        {
            for (int i = 0; i < attrIds.Count(); i++)
            {
                var schemaAttrId = attrIds.ElementAt(i);
                var attr = GetAttributeById(schemaAttrId);
                if (attr == null)
                {
                    continue;
                }

                removedAttrs.Add(attr);
                var children = GetChildren(attr);
                var childrenIds = children.Select(a => a.Id).ToList();
                RemoveAttributesById(removedAttrs, childrenIds);
                Attributes.Remove(attr);
            }
        }

        public SCIMRepresentationAttribute GetAttributeById(string id)
        {
            return Attributes.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetAttributesByPath(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return new SCIMRepresentationAttribute[0];
            }

            return Attributes.Where(a => a.FullPath == fullPath);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetAttributesByAttrSchemaId(string attrSchemaId)
        {
            return Attributes.Where(a => a.SchemaAttributeId == attrSchemaId);
        }

        public SCIMRepresentationAttribute GetParentAttribute(SCIMRepresentationAttribute attr)
        {
            return GetParentAttributeById(attr.ParentAttributeId);
        }

        public SCIMRepresentationAttribute GetParentAttributeById(string id)
        {
            return GetAttributeById(id);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetParentAttributesByPath(SCIMRepresentationAttribute attr)
        {
            return GetParentAttributesByPath(attr.FullPath);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetParentAttributesByPath(string fullPath)
        {
            fullPath = GetParentPath(fullPath);
            return GetAttributesByPath(fullPath);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetChildren(SCIMRepresentationAttribute attr)
        {
            return Attributes.Where(a => a.ParentAttributeId == attr.Id);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetFlatHierarchicalChildren(SCIMRepresentationAttribute attr)
        {
            var result = new List<SCIMRepresentationAttribute>();
            result.Add(attr);
            foreach (var child in GetChildren(attr))
            {
                result.AddRange(GetFlatHierarchicalChildren(child));
            }

            return result;
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
            Version++;
            LastModified = lastModified;
        }

        public void SetVersion(int version)
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

        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public void AddStandardAttributes(string location)
        {
            var metadata = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute(SCIMConstants.StandardSCIMRepresentationAttributes.Meta));
            AddAttribute(metadata);
            AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute($"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.ResourceType}"))
            {
                ValueString = ResourceType
            });
            AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute($"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Created}"))
            {
                ValueDateTime = Created
            });
            AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute($"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.LastModified}"))
            {
                ValueDateTime = LastModified
            });
            AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute($"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Version}"))
            {
                ValueInteger = Version
            });
            if (!string.IsNullOrWhiteSpace(location))
            {
                AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute($"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Location}"))
                {
                    ValueString = location
                });
            }

            if (!string.IsNullOrWhiteSpace(ExternalId))
            {
                AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute(SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId))
                {
                    ValueString = ExternalId
                });
            }

            foreach (var schemaId in Schemas.Select(s => s.Id))
            {
                AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), SCIMConstants.StandardSchemas.StandardResponseSchemas.GetAttribute(SCIMConstants.StandardSCIMRepresentationAttributes.Schemas))
                {
                    ValueString = schemaId
                });
            }
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

        public static List<TreeNode<SCIMRepresentationAttribute>> BuildHierarchicalAttributes(ICollection<SCIMRepresentationAttribute> attributes)
        {
            var rootId = "paul";

            var parentsDictionary = new Dictionary<string, List<TreeNode<SCIMRepresentationAttribute>>>();
            var treeNodes = new List<TreeNode<SCIMRepresentationAttribute>>();

            foreach (var scimRepresentationAttribute in attributes)
            {
                var parentIdKey = scimRepresentationAttribute.ParentAttributeId ?? rootId;
                var treeNode = new TreeNode<SCIMRepresentationAttribute>(scimRepresentationAttribute);
                treeNodes.Add(treeNode);

                if (!parentsDictionary.ContainsKey(parentIdKey))
                {
                    parentsDictionary[parentIdKey] = new List<TreeNode<SCIMRepresentationAttribute>>() { treeNode };
                    continue;
                }

                parentsDictionary[parentIdKey].Add(treeNode);
            }

            foreach (var node in treeNodes.Where(node => parentsDictionary.ContainsKey(node.Leaf.Id)))
            {
                node.Children = parentsDictionary[node.Leaf.Id];
            }

            return parentsDictionary[rootId];
        }

        public static List<SCIMRepresentationAttribute> BuildFlatAttributes(ICollection<TreeNode<SCIMRepresentationAttribute>> attributes)
        {
            return attributes.SelectMany(a => a.ToFlat()).ToList();
        }

        public static string GetParentPath(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return null;
            }

            var splitted = fullPath.Split('.').ToList();
            if (splitted.Count <= 1)
            {
                return null;
            }

            splitted.Remove(splitted.Last());
            return string.Join(".", splitted);
        }

        private static void EnrichAttributesHiearchy(SCIMRepresentationAttribute childAttribute, List<SCIMRepresentationAttribute> flatAttrsHiearchy, IEnumerable<SCIMRepresentationAttribute> allAttributes)
        {
            if (string.IsNullOrWhiteSpace(childAttribute.ParentAttributeId))
            {
                return;
            }

            var parents = allAttributes.Where(a => a.Id == childAttribute.ParentAttributeId);
            flatAttrsHiearchy.AddRange(parents);
            foreach (var parent in parents)
            {
                EnrichAttributesHiearchy(parent, flatAttrsHiearchy, allAttributes);
            }
        }

        private List<TreeNode<SCIMRepresentationAttribute>> BuildHierarchicalAttributes()
        {
            return BuildHierarchicalAttributes(Attributes);
        }
    }
}
