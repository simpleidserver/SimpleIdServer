using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Builder
{
    public class SCIMRepresentationBuilder
    {
        private readonly ICollection<SCIMSchema> _schemas;
        private readonly ICollection<SCIMRepresentationAttribute> _attributes;

        private SCIMRepresentationBuilder(ICollection<SCIMSchema> schemas)
        {
            _schemas = schemas;
            _attributes = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationBuilder AddAttribute(string name, string schemaId, List<int> valuesInt = null, List<bool> valuesBool = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null)
        {
            var schemaAttribute = _schemas.First(s => s.Id == schemaId).Attributes.FirstOrDefault(a => a.Name == name);
            var id = Guid.NewGuid().ToString();
            _attributes.Add(new SCIMRepresentationAttribute(id, schemaAttribute, valuesInt, valuesBool, valuesString, valuesDateTime));
            return this;
        }

        public SCIMRepresentationBuilder AddStringAttribute(string name, string schemaId, List<string> valuesString)
        {
            return AddAttribute(name, schemaId, valuesString: valuesString);
        }

        public SCIMRepresentationBuilder AddBooleanAttribute(string name, string schemaId, List<bool> valuesBoolean)
        {
            return AddAttribute(name, schemaId, valuesBool: valuesBoolean);
        }

        public SCIMRepresentationBuilder AddIntegerAttribute(string name, string schemaId, List<int> valuesInteger)
        {
            return AddAttribute(name, schemaId, valuesInt: valuesInteger);
        }

        public SCIMRepresentationBuilder AddDateTimeAttribute(string name, string schemaId, List<DateTime> valuesDateTime)
        {
            return AddAttribute(name, schemaId, valuesDateTime: valuesDateTime);
        }

        public SCIMRepresentationBuilder AddComplexAttribute(string name, string schemaId, Action<SCIMRepresentationAttributeBuilder> callback)
        {
            var schemaAttribute = _schemas.First(s => s.Id == schemaId).Attributes.FirstOrDefault(a => a.Name == name);
            var builder = new SCIMRepresentationAttributeBuilder(schemaAttribute);
            callback(builder);
            var id = Guid.NewGuid().ToString();
            var newAttribute = new SCIMRepresentationAttribute(id, schemaAttribute);
            foreach(var subAttribute in builder.Build())
            {
                newAttribute.Add(subAttribute);
            }

            _attributes.Add(newAttribute);
            return this;
        }

        public SCIMRepresentation Build()
        {
            return new SCIMRepresentation(_schemas, _attributes);
        }

        public static SCIMRepresentationBuilder Create(ICollection<SCIMSchema> schemas)
        {
            return new SCIMRepresentationBuilder(schemas);
        }
    }
}
