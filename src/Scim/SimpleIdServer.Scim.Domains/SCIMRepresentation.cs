// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SimpleIdServer.Scim.Domains
{
    public class SCIMRepresentation : BaseDomain, IEquatable<SCIMRepresentation>
    {
        public SCIMRepresentation()
        {
            Schemas = new List<SCIMSchema>();
            FlatAttributes = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentation(ICollection<SCIMSchema> schemas, ICollection<SCIMRepresentationAttribute> attributes)
        {
            Schemas = schemas;
            FlatAttributes = attributes;
        }

        public string ExternalId { get; set; }
        public string ResourceType { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public string? RealmName { get; set; } = null;
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<SCIMRepresentationAttribute> FlatAttributes { get; set; }
        public ICollection<SCIMSchema> Schemas { get; set; }
        public ICollection<SCIMRepresentationAttribute> LeafAttributes
        {
            get
            {
                return FlatAttributes.Where(a => a.IsLeaf()).ToList();
            }
        }
        public ICollection<SCIMRepresentationAttribute> HierarchicalAttributes
        {
            get
            {
                return BuildHierarchicalAttributes(FlatAttributes);
            }
        }

        public void RefreshHierarchicalAttributesCache()
        {
            BuildHierarchicalAttributes(FlatAttributes);
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
            attribute.RepresentationId = Id;
            attribute.ComputeValueIndex();
            FlatAttributes.Add(attribute);
        }

        public void UpdateAttribute(SCIMRepresentationAttribute attribute)
        {
            var attr = FlatAttributes.First(a => a.Id == attribute.Id);
            attr.ValueBinary = attribute.ValueBinary;
            attr.ValueBoolean = attribute.ValueBoolean;
            attr.ValueDateTime = attribute.ValueDateTime;
            attr.ValueDecimal = attribute.ValueDecimal;
            attr.ValueInteger = attribute.ValueInteger;
            attr.ValueReference = attribute.ValueReference;
            attr.ValueString = attribute.ValueString;
        }

        public SCIMRepresentation Duplicate()
        {
            return Clone() as SCIMRepresentation;
        }

        public void AddAttribute(SCIMRepresentationAttribute parentAttribute, SCIMRepresentationAttribute childAttribute)
        {
            childAttribute.ParentAttributeId = parentAttribute.Id;
            FlatAttributes.Add(childAttribute);
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
                var attrs = GetAttributesByAttrSchemaId(schemaAttrId).Select(a => a.Id).ToList();
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
                attr.Children?.Clear();
                FlatAttributes.Remove(attr);
                RemoveAttributesById(removedAttrs, childrenIds);
            }
        }

        public SCIMRepresentationAttribute GetAttributeById(string id)
        {
            return FlatAttributes.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetAttributesByPath(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return new SCIMRepresentationAttribute[0];
            }

            return FlatAttributes.Where(a => a.FullPath == fullPath);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetAttributesByAttrSchemaId(string attrSchemaId)
        {
            return FlatAttributes.Where(a => a.SchemaAttributeId == attrSchemaId);
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
            return FlatAttributes.Where(a => a.ParentAttributeId == attr.Id);
        }

        public IEnumerable<SCIMRepresentationAttribute> GetChildren(string id)
        {
            return FlatAttributes.Where(a => a.ParentAttributeId == id);
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

        public void SetUpdated(DateTime lastModified, string version)
        {
            LastModified = lastModified;
            Version = version;
        }

        public void SetResourceType(string resourceType)
        {
            ResourceType = resourceType;
        }

        public bool ContainsAttribute(SCIMRepresentationAttribute attr)
        {
            return FlatAttributes.Any(a => a.IsSimilar(attr));
        }

        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public void AddStandardAttributes(string location, IEnumerable<string> attributes, bool isIncluded = true, bool ignore = true)
        {
            var metadata = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute(StandardSCIMRepresentationAttributes.Meta), StandardSchemas.ResourceTypeSchema.Id);
            var startWithMeta = attributes.Any(a => a.StartsWith(StandardSCIMRepresentationAttributes.Meta) && a.Split('.').Count() > 1);
            var containsMeta = attributes.Contains(StandardSCIMRepresentationAttributes.Meta);
            if (startWithMeta || ignore || !(containsMeta && !isIncluded))
            {
                AddAttribute(metadata);
                var includeAll = containsMeta && isIncluded;
                var containsResourceType = attributes.Contains($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}");
                var containsCreated = attributes.Contains($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}");
                var containsLastModified = attributes.Contains($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}");
                var containsVersion = attributes.Contains($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}");
                var containsLocation = attributes.Contains($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMRepresentationAttributes.Location}");
                if (containsResourceType && isIncluded || !containsResourceType && !isIncluded || ignore || includeAll)
                {
                    AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}"), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueString = ResourceType
                    });
                }

                if (containsCreated && isIncluded || !containsCreated && !isIncluded || ignore || includeAll)
                {
                    AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}"), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueDateTime = Created
                    });
                }

                if (containsLastModified && isIncluded || !containsLastModified && !isIncluded || ignore || includeAll)
                {
                    AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}"), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueDateTime = LastModified
                    });
                }

                if (containsVersion && isIncluded || !containsVersion && !isIncluded || ignore || includeAll)
                {
                    AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}"), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueString = Version
                    });
                }

                if ((containsLocation && isIncluded || !containsLocation && !isIncluded || ignore || includeAll) && !string.IsNullOrWhiteSpace(location))
                {
                    AddAttribute(metadata, new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute($"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Location}"), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueString = location
                    });
                }
            }

            var containsExternalId = attributes.Contains(StandardSCIMRepresentationAttributes.ExternalId);
            var containsSchemas = attributes.Contains(StandardSCIMRepresentationAttributes.Schemas);
            if (containsExternalId && isIncluded || !containsExternalId && !isIncluded || ignore)
            {
                if (!string.IsNullOrWhiteSpace(ExternalId))
                {
                    AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute(StandardSCIMRepresentationAttributes.ExternalId), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueString = ExternalId
                    });
                }
            }

            if (containsSchemas && isIncluded || !containsSchemas && !isIncluded || ignore)
            {
                foreach (var schemaId in Schemas.Select(s => s.Id))
                {
                    AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), StandardSchemas.StandardResponseSchemas.GetAttribute(StandardSCIMRepresentationAttributes.Schemas), StandardSchemas.ResourceTypeSchema.Id)
                    {
                        ValueString = schemaId
                    });
                }
            }
        }

        public override object Clone()
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
                FlatAttributes = FlatAttributes.Select(a => (SCIMRepresentationAttribute)a.Clone()).ToList(),
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
        public static List<SCIMRepresentationAttribute> BuildFlatAttributes(ICollection<SCIMRepresentationAttribute> attributes)
        {
            return attributes.SelectMany(a => a.ToFlat()).ToList();
        }

        public static List<SCIMRepresentationAttribute> BuildHierarchicalAttributes(IEnumerable<SCIMRepresentationAttribute> attributes)
        {
            var rootId = string.Empty;
            if (attributes.Count() == 0) return new List<SCIMRepresentationAttribute>();
            var parentsDictionary = new Dictionary<string, List<SCIMRepresentationAttribute>>();
            var treeNodes = new List<SCIMRepresentationAttribute>();
            foreach (var scimRepresentationAttribute in attributes)
            {
                var parentIdKey = scimRepresentationAttribute.ParentAttributeId ?? rootId;
                treeNodes.Add(scimRepresentationAttribute);
                if (!parentsDictionary.ContainsKey(parentIdKey))
                {
                    parentsDictionary[parentIdKey] = new List<SCIMRepresentationAttribute>() { scimRepresentationAttribute };
                    continue;
                }

                parentsDictionary[parentIdKey].Add(scimRepresentationAttribute);
            }

            foreach(var node in treeNodes)
            {
                if(parentsDictionary.ContainsKey(node.Id))
                {
                    node.CachedChildren = parentsDictionary[node.Id];
                    node.Children = parentsDictionary[node.Id];
                }
                else
                {
                    var lst = new List<SCIMRepresentationAttribute>();
                    node.CachedChildren = lst;
                    node.Children = lst;
                }
            }
            var attrWithNoParentLst = attributes.Where(a => a.ParentAttributeId == rootId || !attributes.Any(c => c.Id == a.ParentAttributeId)).ToList();
            foreach (var attrWithNoParent in attrWithNoParentLst)
            {
                attrWithNoParent.ComputeValueIndex();
            }

            return attrWithNoParentLst;
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
    }
}
