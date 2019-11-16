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

        public SCIMRepresentationAttributeBuilder AddAttribute(string name, List<int> valuesInt = null, List<bool> valuesBool = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null)
        {
            var id = Guid.NewGuid().ToString();
            var schemaAttribute = _scimSchemaAttribute.SubAttributes.FirstOrDefault(a => a.Name == name);
            _attributes.Add(new SCIMRepresentationAttribute(id, schemaAttribute, valuesInt, valuesBool, valuesString, valuesDateTime));
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
