// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleIdServer.Scim.Domains
{
    [DebuggerDisplay("FullPath = {FullPath}, Id = {Id}, ParentAttributeId = {ParentAttributeId}, AttributeId = {AttributeId}")]
    public class SCIMRepresentationAttribute : BaseDomain, IComparable<SCIMRepresentationAttribute>
    {
        private ICollection<string> _computedAttributeNames = new List<string>
        {
            "display", "type"
        };

        public SCIMRepresentationAttribute()
        {
            Children = new List<SCIMRepresentationAttribute>();
            CachedChildren = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttribute(string id, string attributeId) : this()
        {
            Id = id;
            AttributeId = attributeId;
        }

        public SCIMRepresentationAttribute(string id, string attributeId, SCIMSchemaAttribute schemaAttribute, 
            string namespaceStr, 
            int? valueInteger = null, 
            bool? valueBoolean = null, 
            string valueString = null, 
            DateTime? valueDateTime = null,
            decimal? valueDecimal = null,
            string valueBinary = null,
            string valueReference = null) : this(id, attributeId)
        {
            SchemaAttribute = schemaAttribute;
            Namespace = namespaceStr;
            ValueInteger = valueInteger;
            ValueBoolean = valueBoolean;
            ValueString = valueString;
            ValueDateTime = valueDateTime;
            ValueDecimal = valueDecimal;
            ValueBinary = valueBinary;
            ValueReference = valueReference;
            if (schemaAttribute != null)
            {
                FullPath = schemaAttribute.FullPath;
                SchemaAttributeId = schemaAttribute.Id;
            }
        }

        #region Properties

        public string AttributeId { get; set; }
        public string ResourceType { get; set; }
        public string ParentAttributeId { get; set; }
        public string SchemaAttributeId { get; set; }
        public string RepresentationId { get; set; }
        public string FullPath { get; set; }
        public string ValueString { get; set; }
        public bool? ValueBoolean { get; set; }
        public int? ValueInteger { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public string ValueReference { get; set; }
        public decimal? ValueDecimal { get; set; }
        public string ValueBinary { get; set; }
        public string Namespace { get; set; }
        public string ComputedValueIndex { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public SCIMSchemaAttribute SchemaAttribute { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public SCIMRepresentation Representation { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<SCIMRepresentationAttribute> Children { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<SCIMRepresentationAttribute> CachedChildren { get; set; }
        public bool IsComputed { get; set; }

        #endregion

        #region Getters

        public void ComputeValueIndex()
        {
            if (SchemaAttribute.Type != SCIMSchemaAttributeTypes.COMPLEX)
            {
                ComputedValueIndex = GetValueIndex();
                return;
            }

            var lst = new List<string>();
            foreach (var child in Children)
            {
                child.ComputeValueIndex();
                var uniqueKey = child.ComputedValueIndex;
                if (uniqueKey != null) lst.Add(uniqueKey);
            }

            ComputedValueIndex = string.Join(";", lst);
        }

        public string GetValueIndex()
        {
            if (IsComputed) return null;
            string value = null;
            switch (SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.BINARY:
                    value = ValueBinary;
                    break;
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    value = ValueBoolean.ToString();
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    value = ValueDateTime?.ToString();
                    break;
                case SCIMSchemaAttributeTypes.DECIMAL:
                    value = ValueDecimal?.ToString();
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    return ValueInteger?.ToString();
                case SCIMSchemaAttributeTypes.REFERENCE:
                    return ValueReference?.ToString();
                case SCIMSchemaAttributeTypes.STRING:
                    return ValueString?.ToString();
                case SCIMSchemaAttributeTypes.COMPLEX:
                    return null;
            }

            return $"{FullPath}={value}";
        }

        public bool IsReadable(bool isGetRequest = false)
        {
            if (SchemaAttribute.Returned == SCIMSchemaAttributeReturned.NEVER
                || SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.WRITEONLY
                || SchemaAttribute.Returned == SCIMSchemaAttributeReturned.REQUEST && isGetRequest)
            {
                return false;
            }
            
            return true;
        }

        public bool IsMutabilityValid(SCIMRepresentationAttribute attr)
        {
            if(SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.IMMUTABLE)
            {
                return IsSimilar(attr, true);
            }

            if (SchemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                var schemaAttributeIds = CachedChildren.Select(c => c.SchemaAttributeId).Intersect(attr.CachedChildren.Select(c => c.SchemaAttributeId));
                var filteredChildren = CachedChildren.Where(c => schemaAttributeIds.Contains(c.SchemaAttributeId));
                var filteredAttrChildren = attr.CachedChildren.Where(c => schemaAttributeIds.Contains(c.SchemaAttributeId));
                foreach (var child in filteredChildren)
                {
                    if (filteredAttrChildren.All(c => !c.IsMutabilityValid(child))) return false;
                }
            }

            return true;
        }

        public bool IsSimilar(SCIMRepresentationAttribute attr, bool ignoreCheckAttributeId = false)
        {
            if (!ignoreCheckAttributeId && attr.AttributeId != AttributeId)
            {
                return false;
            }

            switch (attr.SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.STRING:
                    return attr.ValueString == ValueString;
                case SCIMSchemaAttributeTypes.BINARY:
                    return attr.ValueBinary == ValueBinary;
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    return attr.ValueBoolean == ValueBoolean;
                case SCIMSchemaAttributeTypes.DATETIME:
                    return attr.ValueDateTime == ValueDateTime;
                case SCIMSchemaAttributeTypes.DECIMAL:
                    return attr.ValueDecimal == ValueDecimal;
                case SCIMSchemaAttributeTypes.INTEGER:
                    return attr.ValueInteger == ValueInteger;
                case SCIMSchemaAttributeTypes.REFERENCE:
                    return attr.ValueReference == ValueReference;
                case SCIMSchemaAttributeTypes.COMPLEX:
                    var schemaAttributeIds = CachedChildren.Select(c => c.SchemaAttributeId).Intersect(attr.CachedChildren.Select(c => c.SchemaAttributeId));
                    var filteredChildren = CachedChildren.Where(c => schemaAttributeIds.Contains(c.SchemaAttributeId));
                    var filteredAttrChildren = attr.CachedChildren.Where(c => schemaAttributeIds.Contains(c.SchemaAttributeId));
                    return filteredChildren.Any() && filteredChildren.All(fc => filteredAttrChildren.Any(sc => fc.IsSimilar(sc, ignoreCheckAttributeId)));
            }

            return false;
        }

        public bool IsLeaf() => IsLeaf(FullPath);

        public int GetLevel() => GetLevel(FullPath);

        public string GetParentFullPath() => GetParentFullPath(FullPath);

        public static bool IsLeaf(string path) => GetLevel(path) == 1;

        public static int GetLevel(string path) => path.Split('.').Length;

        public static string GetParentFullPath(string path) => string.Join(".", path.Split('.').Take(GetLevel(path) - 1));

        public void UpdateValue(string path, SCIMRepresentationAttribute attr)
        {
            var flatAttrs = ToFlat();
            var flatAttr = flatAttrs.Single(a => a.FullPath == path);
            flatAttr.ValueString = attr.ValueString;
            flatAttr.ValueBoolean = attr.ValueBoolean;
            flatAttr.ValueReference = attr.ValueReference;
            flatAttr.ValueInteger = attr.ValueInteger;
            flatAttr.ValueDecimal = attr.ValueDecimal;
            flatAttr.ValueDateTime = attr.ValueDateTime;
            flatAttr.ValueBinary = attr.ValueBinary;
            flatAttr.Children = attr.Children;
            flatAttr.CachedChildren = attr.CachedChildren;
            if (flatAttr.Children != null)
            {
                foreach(var child in flatAttr.Children)
                {
                    child.ParentAttributeId = flatAttr.Id;
                    child.RepresentationId = flatAttr.RepresentationId;
                }
            }

            if(flatAttr.CachedChildren != null)
            {
                foreach(var child in flatAttr.CachedChildren)
                {
                    child.ParentAttributeId = flatAttr.Id;
                    child.RepresentationId = flatAttr.RepresentationId;
                }
            }
        }

        public List<SCIMRepresentationAttribute> ToFlat()
        {
            var result = new List<SCIMRepresentationAttribute>
            {
                this
            };
            result.AddRange(CachedChildren.SelectMany(c => c.ToFlat()));
            return result;
        }

        #endregion

        public override object Clone()
        {
            var result = new SCIMRepresentationAttribute(Id, AttributeId)
            {
                Id = Id,
                AttributeId = AttributeId,
                ValueBinary = ValueBinary,
                ValueBoolean = ValueBoolean,
                ValueDateTime = ValueDateTime,
                ValueDecimal = ValueDecimal,
                ValueInteger = ValueInteger,
                ValueReference = ValueReference,
                ValueString = ValueString,
                SchemaAttribute = (SCIMSchemaAttribute)SchemaAttribute.Clone(),
                FullPath = FullPath,
                ParentAttributeId = ParentAttributeId,
                SchemaAttributeId = SchemaAttributeId,
                Namespace = Namespace,
                RepresentationId = RepresentationId,
                ResourceType = ResourceType,
                ComputedValueIndex = ComputedValueIndex
            };
            return result;
        }

        public int CompareTo(SCIMRepresentationAttribute other)
        {
            switch(SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.BINARY:
                    return ValueBinary.CompareTo(other.ValueBinary);
                case SCIMSchemaAttributeTypes.DATETIME:
                    if (ValueDateTime == null) {
                        return -1;
                    }

                    if (other.ValueDateTime == null)
                    {
                        return 1;
                    }

                    return ValueDateTime.Value.CompareTo(other.ValueDateTime.Value);
                case SCIMSchemaAttributeTypes.DECIMAL:
                    if (ValueDecimal == null)
                    {
                        return -1;
                    }

                    if (other.ValueDecimal == null)
                    {
                        return 1;
                    }

                    return ValueDecimal.Value.CompareTo(other.ValueDecimal.Value);
                case SCIMSchemaAttributeTypes.INTEGER:
                    if (ValueInteger == null)
                    {
                        return -1;
                    }

                    if (other.ValueInteger == null)
                    {
                        return 1;
                    }

                    return ValueInteger.Value.CompareTo(other.ValueInteger.Value);
                case SCIMSchemaAttributeTypes.REFERENCE:
                    return ValueReference.CompareTo(other.ValueReference);
                case SCIMSchemaAttributeTypes.STRING:
                    return ValueString.CompareTo(other.ValueString);
            }

            return 0;
        }
    }
}
