// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Builder
{
    public class SCIMRepresentationBuilder
    {
        private string _id;
        private readonly ICollection<SCIMRepresentationAttribute> _attributes;
        private readonly ICollection<SCIMSchema> _schemas;
        private readonly SCIMRepresentation _representation;

        private SCIMRepresentationBuilder(ICollection<SCIMSchema> schemas)
        {
            _id = Guid.NewGuid().ToString();
            _attributes = new List<SCIMRepresentationAttribute>();
            _schemas = schemas;
            _representation = new SCIMRepresentation(_schemas, _attributes)
            {
                Id = _id
            };
        }

        private SCIMRepresentationBuilder(SCIMRepresentation scimRepresentation, ICollection<SCIMSchema> schemas)
        {
            _id = scimRepresentation.Id;
            _attributes = scimRepresentation.FlatAttributes;
            _schemas = schemas;
            _representation = scimRepresentation;
        }

        public SCIMRepresentationBuilder AddAttribute(string name, string schemaId, List<int> valuesInt = null, List<bool> valuesBool = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null, List<decimal> valuesDecimal = null, List<string> valuesBinary = null)
        {
            var schema = _schemas.First(s => s.Id == schemaId);
            var schemaAttribute = schema.Attributes.FirstOrDefault(a => a.Name == name);
            var id = Guid.NewGuid().ToString();
            var attributeId = Guid.NewGuid().ToString();
            if (valuesInt != null)
            {
                foreach (var attr in valuesInt)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaId, valueInteger: attr));
                }
            }
            else if (valuesBool != null)
            {
                foreach (var attr in valuesBool)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaId, valueBoolean: attr));
                }
            }
            else if (valuesString != null)
            {
                foreach (var attr in valuesString)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaId, valueString: attr));
                }
            }
            else if (valuesDateTime != null)
            {
                foreach (var attr in valuesDateTime)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaId, valueDateTime: attr));
                }
            }
            else if (valuesDecimal != null)
            {
                foreach (var attr in valuesDecimal)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaId, valueDecimal: attr));
                }
            }
            else if (valuesBinary != null)
            {
                foreach (var attr in valuesBinary)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaId, valueBinary: attr));
                }
            }

            return this;
        }

        public SCIMRepresentationBuilder AddStringAttribute(string name, string schemaId, List<string> valuesString)
        {
            return AddAttribute(name, schemaId, valuesString: valuesString);
        }

        public SCIMRepresentationBuilder AddDecimalAttribute(string name, string schemaId, List<decimal> valuesDecimal)
        {
            return AddAttribute(name, schemaId, valuesDecimal: valuesDecimal);
        }

        public SCIMRepresentationBuilder AddBinaryAttribute(string name, string schemaId, List<string> valuesBinary)
        {
            return AddAttribute(name, schemaId, valuesBinary: valuesBinary);
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
            var id = Guid.NewGuid().ToString();
            var schema = _schemas.First(s => s.Id == schemaId);
            var schemaAttribute = schema.Attributes.FirstOrDefault(a => a.Name == name);
            var builder = new SCIMRepresentationAttributeBuilder(id, schema, schemaAttribute);
            callback(builder);
            var newAttribute = new SCIMRepresentationAttribute(id, Guid.NewGuid().ToString(), schemaAttribute, schemaId);
            foreach(var subAttribute in builder.Build())
            {
                _representation.AddAttribute(subAttribute);
            }

            _attributes.Add(newAttribute);
            return this;
        }

        public SCIMRepresentation Build()
        {
            return _representation;
        }

        public static SCIMRepresentationBuilder Create(ICollection<SCIMSchema> schemas)
        {
            return new SCIMRepresentationBuilder(schemas);
        }

        public static SCIMRepresentationBuilder Load(SCIMRepresentation scimRepresentation, ICollection<SCIMSchema> schemas)
        {
            return new SCIMRepresentationBuilder((SCIMRepresentation)scimRepresentation.Clone(), schemas);
        }
    }
}
