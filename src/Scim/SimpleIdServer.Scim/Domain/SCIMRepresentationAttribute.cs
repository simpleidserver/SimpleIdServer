using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMRepresentationAttribute : ICloneable
    {
        private ICollection<int> _valuesInteger;
        private ICollection<bool> _valuesBoolean;
        private ICollection<string> _valuesString;
        private ICollection<DateTime> _valuesDateTime;

        public SCIMRepresentationAttribute()
        {
            _valuesInteger = new List<int>();
            _valuesBoolean = new List<bool>();
            _valuesString = new List<string>();
            _valuesDateTime = new List<DateTime>();
            ValuesReference = new List<string>();
            Values = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttribute(SCIMSchemaAttribute schemaAttribute) : this()
        {
            SchemaAttribute = schemaAttribute;
        }

        public SCIMRepresentationAttribute(SCIMSchemaAttribute scimSchemaAttribute, ICollection<SCIMRepresentationAttribute> values) : this(scimSchemaAttribute)
        {
            Values = values;
        }

        public SCIMRepresentationAttribute(SCIMSchemaAttribute scimSchemaAttribute, List<int> valuesInteger, List<bool> valuesBoolean, List<string> valuesString, List<DateTime> valuesDateTime) : this(scimSchemaAttribute)
        {
            _valuesInteger = valuesInteger;
            _valuesBoolean = valuesBoolean;
            _valuesString = valuesString;
            _valuesDateTime = valuesDateTime;
        }

        public string Id { get; set; }
        public IQueryable<string> ValuesString { get => _valuesString.AsQueryable(); }
        public virtual IQueryable<bool> ValuesBoolean { get => _valuesBoolean.AsQueryable(); }
        public virtual IQueryable<int> ValuesInteger { get => _valuesInteger.AsQueryable(); }
        public virtual IQueryable<DateTime> ValuesDateTime { get => _valuesDateTime.AsQueryable(); }
        public ICollection<string> ValuesReference { get; set; }
        public ICollection<SCIMRepresentationAttribute> Values { get; set; }
        public SCIMSchemaAttribute SchemaAttribute { get; set; }

        public void Add(int value)
        {
            _valuesInteger.Add(value);
        }

        public void Add(string value)
        {
            _valuesString.Add(value);
        }

        public void Add(DateTime value)
        {
            _valuesDateTime.Add(value);
        }

        public void Add(bool value)
        {
            _valuesBoolean.Add(value);
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
            return new SCIMRepresentationAttribute
            {
                Id = Id,
                _valuesString = _valuesString.ToList(),
                _valuesBoolean = _valuesBoolean.ToList(),
                _valuesDateTime = _valuesDateTime.ToList(),
                _valuesInteger = _valuesInteger.ToList(),
                ValuesReference = ValuesReference.ToList(),
                Values = Values.Select(v => (SCIMRepresentationAttribute)v.Clone()).ToList(),
                SchemaAttribute = (SCIMSchemaAttribute)SchemaAttribute.Clone()
            };
        }
    }
}
