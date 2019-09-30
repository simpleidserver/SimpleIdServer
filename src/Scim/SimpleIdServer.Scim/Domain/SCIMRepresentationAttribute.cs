using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMRepresentationAttribute : ICloneable
    {
        public SCIMRepresentationAttribute()
        {
            ValuesString = new List<string>();
            ValuesBoolean = new List<bool>();
            ValuesInteger = new List<int>();
            ValuesDateTime = new List<DateTime>();
            ValuesReference = new List<string>();
            Values = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttribute(SCIMSchemaAttribute schemaAttribute) : this()
        {
            SchemaAttribute = schemaAttribute;
        }

        public string Id { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public ICollection<bool> ValuesBoolean { get; set; }
        public ICollection<int> ValuesInteger { get; set; }
        public ICollection<DateTime> ValuesDateTime { get; set; }
        public ICollection<string> ValuesReference { get; set; }
        public ICollection<SCIMRepresentationAttribute> Values { get; set; }
        public SCIMSchemaAttribute SchemaAttribute { get; set; }

        public object Clone()
        {
            return new SCIMRepresentationAttribute
            {
                Id = Id,
                ValuesString = ValuesString.ToList(),
                ValuesBoolean = ValuesBoolean.ToList(),
                ValuesDateTime = ValuesDateTime.ToList(),
                ValuesReference = ValuesReference.ToList(),
                Values = Values.Select(v => (SCIMRepresentationAttribute)v.Clone()).ToList(),
                SchemaAttribute = (SCIMSchemaAttribute)SchemaAttribute.Clone()
            };
        }
    }
}
