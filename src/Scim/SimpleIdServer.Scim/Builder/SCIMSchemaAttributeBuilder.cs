using SimpleIdServer.Scim.Domain;
using System;

namespace SimpleIdServer.Scim.Builder
{
    public class SCIMSchemaAttributeBuilder
    {
        private readonly SCIMSchemaAttribute _scimSchemaAttribute;

        public SCIMSchemaAttributeBuilder(SCIMSchemaAttribute scimSchemaAttribute)
        {
            _scimSchemaAttribute = scimSchemaAttribute;
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
            var builder = new SCIMSchemaAttributeBuilder(new SCIMSchemaAttribute(Guid.NewGuid().ToString()) { Name = name });
            callback(builder);
            _scimSchemaAttribute.SubAttributes.Add(builder.Build());
            return this;
        }

        public SCIMSchemaAttributeBuilder AddAttribute(string name, SCIMSchemaAttributeTypes type, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
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
                Description = description
            });
            if (callback != null)
            {
                callback(builder);
            }

            _scimSchemaAttribute.SubAttributes.Add(builder.Build());
            return this;
        }

        public SCIMSchemaAttributeBuilder AddStringAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.STRING, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddBooleanAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.BOOLEAN, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddDateTimeAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.DATETIME, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddIntAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.INTEGER, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        public SCIMSchemaAttributeBuilder AddComplexAttribute(string name, Action<SCIMSchemaAttributeBuilder> callback = null, bool caseExact = false, bool required = false,
            SCIMSchemaAttributeMutabilities mutability = SCIMSchemaAttributeMutabilities.READWRITE,
            SCIMSchemaAttributeReturned returned = SCIMSchemaAttributeReturned.DEFAULT,
            SCIMSchemaAttributeUniqueness uniqueness = SCIMSchemaAttributeUniqueness.NONE, string description = null, bool multiValued = false)
        {
            return AddAttribute(name, SCIMSchemaAttributeTypes.COMPLEX, callback, caseExact, required, mutability, returned, uniqueness, description, multiValued);
        }

        internal SCIMSchemaAttribute Build()
        {
            return _scimSchemaAttribute;
        }
    }
}
