// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Domains.Builders
{
    public class SCIMSchemaAttributeBuilder
    {
        private readonly SCIMSchema _schema;
        private readonly SCIMSchemaAttribute _scimSchemaAttribute;
        private readonly string _fullPath;
        private readonly string _parentId;

        public SCIMSchemaAttributeBuilder(string parentId, string fullPath, SCIMSchema schema, SCIMSchemaAttribute scimSchemaAttribute)
        {
            _schema = schema;
            _scimSchemaAttribute = scimSchemaAttribute;
            _fullPath = fullPath;
            _parentId = parentId;
        }

        public SCIMSchemaAttributeBuilder SetType(SCIMSchemaAttributeTypes type)
        {
            _scimSchemaAttribute.Type = type;
            return this;
        }

        public SCIMSchemaAttributeBuilder SetMultiValued(bool mutliValued)
        {
            _scimSchemaAttribute.MultiValued = mutliValued;
            return this;
        }

        public SCIMSchemaAttributeBuilder SetRequired(bool required)
        {
            _scimSchemaAttribute.Required = required;
            return this;
        }

        public SCIMSchemaAttributeBuilder SetCaseExact(bool caseExact)
        {
            _scimSchemaAttribute.CaseExact = caseExact;
            return this;
        }

        public SCIMSchemaAttributeBuilder SetMutability(SCIMSchemaAttributeMutabilities mutability)
        {
            _scimSchemaAttribute.Mutability = mutability;
            return this;
        }

        public SCIMSchemaAttributeBuilder SetReturned(SCIMSchemaAttributeReturned returned)
        {
            _scimSchemaAttribute.Returned = returned;
            return this;
        }

        public SCIMSchemaAttributeBuilder SetUniqueness(SCIMSchemaAttributeUniqueness uniqueness)
        {
            _scimSchemaAttribute.Uniqueness = uniqueness;
            return this;
        }

        public SCIMSchemaAttributeBuilder AddAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback)
        {
            var fullPath = $"{_fullPath}.{name}";
            var builder = new SCIMSchemaAttributeBuilder(_scimSchemaAttribute.Id, fullPath, _schema, new SCIMSchemaAttribute(Guid.NewGuid().ToString()) { Name = name });
            callback(builder);
            _schema.AddAttribute(_scimSchemaAttribute, builder.Build());
            return this;
        }

        public SCIMSchemaAttributeBuilder AddAttribute(SCIMSchema schema, string name, SCIMSchemaAttributeTypes type, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false, List<string> canonicalValues = null)
        {
            var fullPath = $"{_fullPath}.{name}";
            var builder = new SCIMSchemaAttributeBuilder(_scimSchemaAttribute.Id, fullPath, schema, new SCIMSchemaAttribute(Guid.NewGuid().ToString())
            {
                Name = name,
                MultiValued = multiValued,
                CaseExact = caseExact,
                Required = required,
                Mutability = mutability,
                Returned = returned,
                Uniqueness = uniqueness,
                Type = type,
                Description = description,
                CanonicalValues = canonicalValues,
            });
            if (callback != null)
            {
                callback(builder);
            }

            _schema.AddAttribute(builder.Build());
            return this;
        }

        public SCIMSchemaAttributeBuilder AddStringAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.STRING, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddDecimalAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.DECIMAL, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddBinaryAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.BINARY, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddBooleanAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.BOOLEAN, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddDateTimeAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.DATETIME, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddIntAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.INTEGER, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddComplexAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(_schema, name, SCIMSchemaAttributeTypes.COMPLEX, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        internal SCIMSchemaAttribute Build()
        {
            var result = _scimSchemaAttribute;
            result.FullPath = _fullPath;
            result.ParentId = _parentId;
            return result;
        }
    }
}
