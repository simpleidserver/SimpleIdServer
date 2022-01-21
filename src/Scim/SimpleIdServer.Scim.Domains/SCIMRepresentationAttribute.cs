// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleIdServer.Scim.Domains
{
    [DebuggerDisplay("FullPath = {FullPath}, Id = {Id}, ParentAttributeId = {ParentAttributeId}, AttributeId = {AttributeId}")]
    public class SCIMRepresentationAttribute : BaseDomain, ICloneable, IComparable<SCIMRepresentationAttribute>
    {
        public SCIMRepresentationAttribute()
        {
            Children = new List<SCIMRepresentationAttribute>();
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
        public SCIMSchemaAttribute SchemaAttribute { get; set; }
        public SCIMRepresentation Representation { get; set; }
        public ICollection<SCIMRepresentationAttribute> Children { get; set; }

        #endregion

        #region Getters

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

        public bool IsSimilar(SCIMRepresentationAttribute attr)
        {
            if (attr.AttributeId != AttributeId)
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
            }

            return false;
        }

        public bool IsLeaf()
        {
            return GetLevel() == 1;
        }

        public int GetLevel()
        {
            return FullPath.Split('.').Length;
        }

        public List<SCIMRepresentationAttribute> ToFlat()
        {
            var result = new List<SCIMRepresentationAttribute>
            {
                this
            };
            result.AddRange(Children.SelectMany(c => c.ToFlat()));
            return result;
        }

        #endregion

        public object Clone()
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
                ResourceType = ResourceType
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
