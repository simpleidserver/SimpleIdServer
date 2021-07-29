// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Diagnostics;

namespace SimpleIdServer.Scim.Domain
{
    [DebuggerDisplay("FullPath = {FullPath}, Id = {Id}, ParentAttributeId = {ParentAttributeId}, AttributeId = {AttributeId}")]
    public class SCIMRepresentationAttribute : ICloneable
    {
        public SCIMRepresentationAttribute()
        {
        }

        public SCIMRepresentationAttribute(string id, string attributeId) : this()
        {
            Id = id;
            AttributeId = attributeId;
        }

        public SCIMRepresentationAttribute(string id, string attributeId, SCIMSchemaAttribute schemaAttribute, int? valueInteger = null, 
            bool? valueBoolean = null, 
            string valueString = null, 
            DateTime? valueDateTime = null,
            decimal? valueDecimal = null,
            byte[] valueBinary = null,
            string valueReference = null) : this(id, attributeId)
        {
            SchemaAttribute = schemaAttribute;
            FullPath = schemaAttribute.FullPath;
            ValueInteger = valueInteger;
            ValueBoolean = valueBoolean;
            ValueString = valueString;
            ValueDateTime = valueDateTime;
            ValueDecimal = valueDecimal;
            ValueBinary = valueBinary;
            ValueReference = valueReference;
        }

        public string Id { get; set; }
        public string AttributeId { get; set; }
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
        public byte[] ValueBinary { get; set; }
        public SCIMSchemaAttribute SchemaAttribute { get; set; }
        public SCIMRepresentation Representation { get; set; }

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

        /*
        public bool IsSimilar(SCIMRepresentationAttribute attr)
        {
            if (attr.SchemaAttribute.Name != SchemaAttribute.Name)
            {
                return false;
            }

            switch (attr.SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.STRING:
                    return attr.ValueString == 
                    if (attr.ValuesString.All(s => ValuesString.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.BINARY:
                    if (attr.ValuesBinary.All(s => ValuesBinary.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    if (attr.ValuesBoolean.All(s => ValuesBoolean.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    if (attr.ValuesDateTime.All(s => ValuesDateTime.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.DECIMAL:
                    if (attr.ValuesDecimal.All(s => ValuesDecimal.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    if (attr.ValuesInteger.All(s => ValuesInteger.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.REFERENCE:
                    if (attr.ValuesReference.All(s => ValuesReference.Contains(s)))
                    {
                        return true;
                    }
                    break;
                case SCIMSchemaAttributeTypes.COMPLEX:
                    if (attr.Values.All(s => Values.Any(v => v.IsSimilar(s))))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
        */

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
                SchemaAttributeId = SchemaAttributeId
            };
            return result;
        }
    }
}
