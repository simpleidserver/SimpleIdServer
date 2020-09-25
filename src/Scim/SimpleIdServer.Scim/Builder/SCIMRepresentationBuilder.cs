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

        private SCIMRepresentationBuilder(ICollection<SCIMSchema> schemas)
        {
            _id = Guid.NewGuid().ToString();
            _attributes = new List<SCIMRepresentationAttribute>();
            _schemas = schemas;
        }

        private SCIMRepresentationBuilder(SCIMRepresentation scimRepresentation, ICollection<SCIMSchema> schemas)
        {
            _id = scimRepresentation.Id;
            _attributes = scimRepresentation.Attributes;
            _schemas = schemas;
        }

        public SCIMRepresentationBuilder AddAttribute(string name, string schemaId, List<int> valuesInt = null, List<bool> valuesBool = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null, List<decimal> valuesDecimal = null, List<byte[]> valuesBinary = null)
        {
            var schemaAttribute = _schemas.First(s => s.Id == schemaId).Attributes.FirstOrDefault(a => a.Name == name);
            var id = Guid.NewGuid().ToString();
            _attributes.Add(new SCIMRepresentationAttribute(id, schemaAttribute, valuesInt, valuesBool, valuesString, valuesDateTime, valuesDecimal, valuesBinary));
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
            return AddAttribute(name, schemaId, valuesBinary: valuesBinary.Select(_ => Convert.FromBase64String(_)).ToList());
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

        public SCIMRepresentationBuilder AddComplexAttribute(string name, Action<SCIMRepresentationAttributeBuilder> callback)
        {
            var builder = new SCIMRepresentationAttributeBuilder(null);
            callback(builder);
            var id = Guid.NewGuid().ToString();
            var newAttribute = new SCIMRepresentationAttribute(id, null);
            foreach (var subAttribute in builder.Build())
            {
                newAttribute.Add(subAttribute);
            }

            _attributes.Add(newAttribute);
            return this;
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
            return new SCIMRepresentation(_schemas, _attributes)
            {
                Id = _id
            };
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
