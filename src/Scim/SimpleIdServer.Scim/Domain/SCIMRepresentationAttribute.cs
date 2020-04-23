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
            Values = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttribute(string id) : this()
        {
            Id = id;
        }

        public SCIMRepresentationAttribute(string id, SCIMSchemaAttribute schemaAttribute, List<int> valuesInteger = null, List<bool> valuesBoolean = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null) : this(id)
        {
            SchemaAttribute = schemaAttribute;
            ValuesInteger = valuesInteger == null ? new List<int>() : valuesInteger;
            ValuesBoolean = valuesBoolean == null ? new List<bool>() : valuesBoolean;
            ValuesString = valuesString == null ? new List<string>() : valuesString;
            ValuesDateTime = valuesDateTime == null ? new List<DateTime>() : valuesDateTime;
        }

        public string Id { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public ICollection<bool> ValuesBoolean { get; set; }
        public ICollection<int> ValuesInteger { get; set; }
        public ICollection<DateTime> ValuesDateTime { get; set; }
        public ICollection<string> ValuesReference { get; set; }
        public ICollection<SCIMRepresentationAttribute> Values { get; set; }
        public SCIMRepresentationAttribute Parent { get; set; }
        public SCIMSchemaAttribute SchemaAttribute { get; set; }

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
