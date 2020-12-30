// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Builder
{
    public class SCIMRepresentationAttributeBuilder
    {
        private readonly SCIMSchemaAttribute _scimSchemaAttribute;
        private readonly ICollection<SCIMRepresentationAttribute> _attributes;

        public SCIMRepresentationAttributeBuilder(SCIMSchemaAttribute scimSchemaAttribute)
        {
            _scimSchemaAttribute = scimSchemaAttribute;
            _attributes = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttributeBuilder AddAttribute(string name, List<int> valuesInt = null, List<bool> valuesBool = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null, List<decimal> valuesDecimal = null, List<byte[]> valuesBinary = null)
        {
            var id = Guid.NewGuid().ToString();
            SCIMSchemaAttribute schemaAttribute = null;
            if (_scimSchemaAttribute != null && _scimSchemaAttribute.SubAttributes != null)
            {
                schemaAttribute = _scimSchemaAttribute.SubAttributes.FirstOrDefault(a => a.Name == name);
            }

            _attributes.Add(new SCIMRepresentationAttribute(id, schemaAttribute, valuesInt, valuesBool, valuesString, valuesDateTime, valuesDecimal, valuesBinary));
            return this;
        }

        public SCIMRepresentationAttributeBuilder AddStringAttribute(string name, List<string> valuesString)
        {
            return AddAttribute(name, valuesString: valuesString);
        }

        public SCIMRepresentationAttributeBuilder AddBooleanAttribute(string name, List<bool> valuesBoolean)
        {
            return AddAttribute(name, valuesBool: valuesBoolean);
        }

        public SCIMRepresentationAttributeBuilder AddIntegerAttribute(string name, List<int> valuesInteger)
        {
            return AddAttribute(name,  valuesInt: valuesInteger);
        }

        public SCIMRepresentationAttributeBuilder AddDecimalAttribute(string name, List<decimal> valuesDecimal)
        {
            return AddAttribute(name, valuesDecimal: valuesDecimal);
        }

        public SCIMRepresentationAttributeBuilder AddBinaryAttribute(string name, List<byte[]> valuesBinary)
        {
            return AddAttribute(name, valuesBinary: valuesBinary);
        }

        public SCIMRepresentationAttributeBuilder AddDateTimeAttribute(string name, List<DateTime> valuesDateTime)
        {
            return AddAttribute(name, valuesDateTime: valuesDateTime);
        }

        public SCIMRepresentationAttributeBuilder AddComplexAttribute(string name, string schemaId, Action<SCIMRepresentationAttributeBuilder> callback)
        {
            var schemaAttribute = _scimSchemaAttribute.SubAttributes.FirstOrDefault(a => a.Name == name);
            var builder = new SCIMRepresentationAttributeBuilder(schemaAttribute);
            callback(builder);
            return this;
        }

        public ICollection<SCIMRepresentationAttribute> Build()
        {
            return _attributes;
        }
    }
}
