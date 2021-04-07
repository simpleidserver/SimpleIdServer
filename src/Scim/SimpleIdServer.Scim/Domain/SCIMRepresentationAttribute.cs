// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    [DebuggerDisplay("Attribute = {SchemaAttribute.Name}")]
    public class SCIMRepresentationAttribute : ICloneable
    {
        public SCIMRepresentationAttribute()
        {
            ValuesInteger = new List<int>();
            ValuesBoolean = new List<bool>();
            ValuesString = new List<string>();
            ValuesDateTime = new List<DateTime>();
            ValuesReference = new List<string>();
            ValuesDecimal = new List<decimal>();
            ValuesBinary = new List<byte[]>();
            Values = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttribute(string id) : this()
        {
            Id = id;
        }

        public SCIMRepresentationAttribute(string id, SCIMSchemaAttribute schemaAttribute, List<int> valuesInteger = null, 
            List<bool> valuesBoolean = null, 
            List<string> valuesString = null, 
            List<DateTime> valuesDateTime = null,
            List<decimal> valuesDecimal = null,
            List<byte[]> valuesBinary = null) : this(id)
        {
            SchemaAttribute = schemaAttribute;
            ValuesInteger = valuesInteger == null ? new List<int>() : valuesInteger;
            ValuesBoolean = valuesBoolean == null ? new List<bool>() : valuesBoolean;
            ValuesString = valuesString == null ? new List<string>() : valuesString;
            ValuesDateTime = valuesDateTime == null ? new List<DateTime>() : valuesDateTime;
            ValuesDecimal = valuesDecimal == null ? new List<decimal>() : valuesDecimal;
            ValuesBinary = valuesBinary == null ? new List<byte[]>() : valuesBinary;
        }

        public string Id { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public ICollection<bool> ValuesBoolean { get; set; }
        public ICollection<int> ValuesInteger { get; set; }
        public ICollection<DateTime> ValuesDateTime { get; set; }
        public ICollection<string> ValuesReference { get; set; }
        public ICollection<decimal> ValuesDecimal { get; set; }
        public ICollection<byte[]> ValuesBinary { get; set; }
        public ICollection<SCIMRepresentationAttribute> Values { get; set; }
        public SCIMRepresentationAttribute Parent { get; set; }
        public SCIMSchemaAttribute SchemaAttribute { get; set; }

        public string GetFullPath()
        {
            var lst = new List<string>();
            GetFullPath(lst);
            lst.Reverse();
            return string.Join(".", lst);
        }

        public void GetFullPath(List<string> lst)
        {
            lst.Add(SchemaAttribute.Name);
            if (Parent != null)
            {
                Parent.GetFullPath(lst);
            }
        }

        public void Add(int value)
        {
            ValuesInteger.Add(value);
        }

        public void Add(string value)
        {
            ValuesString.Add(value);
        }

        public void Add(DateTime value)
        {
            ValuesDateTime.Add(value);
        }

        public void Add(bool value)
        {
            ValuesBoolean.Add(value);
        }

        public void Add(decimal value)
        {
            ValuesDecimal.Add(value);
        }

        public void Add(byte[] value)
        {
            ValuesBinary.Add(value);
        }

        public void Add(SCIMRepresentationAttribute value)
        {
            Values.Add(value);
            value.Parent = this;
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
        
        public bool HasValue
        {
            get
            {
                if (Values.Any()
                    || ValuesBinary.Any()
                    || ValuesBoolean.Any()
                    || ValuesDateTime.Any()
                    || ValuesDecimal.Any()
                    || ValuesInteger.Any()
                    || ValuesReference.Any()
                    || ValuesString.Any())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsSimilar(SCIMRepresentationAttribute attr)
        {
            if (attr.SchemaAttribute.Name != SchemaAttribute.Name)
            {
                return false;
            }

            switch(attr.SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.STRING:
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

        public object Clone()
        {
            var result = new SCIMRepresentationAttribute(Id)
            {
                Id = Id,
                ValuesString = ValuesString.ToList(),
                ValuesBoolean = ValuesBoolean.ToList(),
                ValuesDateTime = ValuesDateTime.ToList(),
                ValuesInteger = ValuesInteger.ToList(),
                ValuesReference = ValuesReference.ToList(),
                ValuesDecimal = ValuesDecimal.ToList(),
                ValuesBinary = ValuesBinary.ToList(),
                SchemaAttribute = (SCIMSchemaAttribute)SchemaAttribute.Clone()
            };
            foreach(var cloneAttribute in Values.Select(v => (SCIMRepresentationAttribute)v.Clone()).ToList())
            {
                result.Add(cloneAttribute);
            }

            return result;
        }
    }
}
