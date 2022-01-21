// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domains.Builders
{
    public class SCIMRepresentationAttributeBuilder
    {
        private readonly SCIMSchema _schema;
        private readonly string _parentId;
        private readonly SCIMSchemaAttribute _scimSchemaAttribute;
        private readonly ICollection<SCIMRepresentationAttribute> _attributes;

        public SCIMRepresentationAttributeBuilder(string parentId, SCIMSchema schema, SCIMSchemaAttribute scimSchemaAttribute)
        {
            _schema = schema;
            _parentId = parentId;
            _scimSchemaAttribute = scimSchemaAttribute;
            _attributes = new List<SCIMRepresentationAttribute>();
        }

        public SCIMRepresentationAttributeBuilder AddAttribute(string name, List<int> valuesInt = null, List<bool> valuesBool = null, List<string> valuesString = null, List<DateTime> valuesDateTime = null, List<decimal> valuesDecimal = null, List<string> valuesBinary = null)
        {
            var id = Guid.NewGuid().ToString();
            SCIMSchemaAttribute schemaAttribute = null;
            var subAttributes = _schema.GetChildren(_scimSchemaAttribute);
            if (subAttributes != null)
            {
                schemaAttribute = subAttributes.FirstOrDefault(a => a.Name == name);
            }

            var attributeId = Guid.NewGuid().ToString();
            if (valuesInt != null)
            {
                foreach(var attr in valuesInt)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueInteger: attr)
                    {
                        ParentAttributeId = _parentId
                    });
                }
            }
            else if (valuesBool != null)
            {
                foreach (var attr in valuesBool)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueBoolean: attr)
                    {
                        ParentAttributeId = _parentId
                    });
                }
            }
            else if (valuesString != null)
            {
                foreach (var attr in valuesString)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueString: attr)
                    {
                        ParentAttributeId = _parentId
                    });
                }
            }
            else if (valuesDateTime != null)
            {
                foreach (var attr in valuesDateTime)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueDateTime: attr)
                    {
                        ParentAttributeId = _parentId
                    });
                }
            }
            else if (valuesDecimal != null)
            {
                foreach (var attr in valuesDecimal)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueDecimal: attr)
                    {
                        ParentAttributeId = _parentId
                    });
                }
            }
            else if (valuesBinary != null)
            {
                foreach (var attr in valuesBinary)
                {
                    _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueBinary: attr)
                    {
                        ParentAttributeId = _parentId
                    });
                }
            }

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

        public SCIMRepresentationAttributeBuilder AddBinaryAttribute(string name, List<string> valuesBinary)
        {
            return AddAttribute(name, valuesBinary: valuesBinary);
        }

        public SCIMRepresentationAttributeBuilder AddDateTimeAttribute(string name, List<DateTime> valuesDateTime)
        {
            return AddAttribute(name, valuesDateTime: valuesDateTime);
        }

        public SCIMRepresentationAttributeBuilder AddComplexAttribute(string name, Action<SCIMRepresentationAttributeBuilder> callback)
        {
            var schemaAttribute = _schema.GetChildren(_scimSchemaAttribute).FirstOrDefault(a => a.Name == name);
            var id = Guid.NewGuid().ToString();
            var attributeId = Guid.NewGuid().ToString();
            _attributes.Add(new SCIMRepresentationAttribute(id, attributeId, schemaAttribute, schemaAttribute.SchemaId, valueInteger: 0)
            {
                ParentAttributeId = _parentId
            });
            var builder = new SCIMRepresentationAttributeBuilder(id, _schema, schemaAttribute);
            callback(builder);
            foreach(var newAttr in builder.Build())
            {
                _attributes.Add(newAttr);
            }

            return this;
        }

        public ICollection<SCIMRepresentationAttribute> Build()
        {
            return _attributes;
        }
    }
}
