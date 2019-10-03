using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Builder
{
    public class SCIMSchemaBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _description;
        private readonly ICollection<SCIMSchemaAttribute> _attributes;

        public SCIMSchemaBuilder(string id)
        {
            _id = id;
            _attributes = new List<SCIMSchemaAttribute>();
        }

        public SCIMSchemaBuilder(string id, string name) : this(id)
        {
            _name = name;
        }
        
        public SCIMSchemaBuilder(string id, string name, string description) : this(id)
        {
            _description = description;
        }

        public SCIMSchemaBuilder AddAttribute(string name)
        {
            _attributes.Add(new SCIMSchemaAttribute(Guid.NewGuid().ToString()) { Name = name });
            return this;
        }

        public SCIMSchemaBuilder AddAttribute(string name, SCIMSchemaAttributeTypes type, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null,
            bool multiValued = false, ICollection<string> defaulValueStr = null, ICollection<int> defaultValueInt = null)
        {
            var builder = new SCIMSchemaAttributeBuilder(new SCIMSchemaAttribute(Guid.NewGuid().ToString())
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
                DefaultValueInt = defaultValueInt,
                DefaultValueString = defaulValueStr
            });
            if (callback != null)
            {
                callback(builder);
            }

            _attributes.Add(builder.Build());
            return this;
        }

        public SCIMSchemaBuilder AddStringAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, ICollection<string> defaultValue = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.STRING, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued, defaulValueStr: defaultValue);
        }

        public SCIMSchemaBuilder AddBooleanAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.BOOLEAN, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaBuilder AddDateTimeAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.DATETIME, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaBuilder AddIntAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, ICollection<int> defaultValue = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.INTEGER, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued, defaultValueInt: defaultValue);
        }

        public SCIMSchemaBuilder AddComplexAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.COMPLEX, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaBuilder AddAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback)
        {
            var builder = new SCIMSchemaAttributeBuilder(new SCIMSchemaAttribute(Guid.NewGuid().ToString()) { Name = name });
            callback(builder);
            _attributes.Add(builder.Build());
            return this;
        }


        public SCIMSchema Build()
        {
            return new SCIMSchema
            {
                Id = _id,
                Name = _name,
                Description = _description,
                Attributes = _attributes
            };
        }

        public static SCIMSchemaBuilder Create(string id)
        {
            var result = new SCIMSchemaBuilder(id);
            return result;
        }

        public static SCIMSchemaBuilder Create(string id, string name)
        {
            var result = new SCIMSchemaBuilder(id, name);
            return result;
        }

        public static SCIMSchemaBuilder Create(string id, string name, string description)
        {
            var result = new SCIMSchemaBuilder(id, name, description);
            return result;
        }
    }
}