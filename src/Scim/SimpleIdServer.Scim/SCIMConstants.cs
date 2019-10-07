using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim
{
    public static class SCIMConstants
    {
        public static class StandardSCIMRepresentationAttributes
        {
            public const string Schemas = "schemas";
            public const string Meta = "meta";
            public const string Id = "id";
            public const string ExternalId = "externalId";
            public const string TotalResults = "totalResults";
            public const string StartIndex = "startIndex";
            public const string ItemsPerPage = "itemsPerPage";
        }

        public static class StandardSCIMMetaAttributes
        {
            public const string ResourceType = "resourceType";
            public const string Created = "created";
            public const string LastModified = "lastModified";
            public const string Location = "location";
            public const string Version = "version";
        }

        public static class StandardSCIMSearchAttributes
        {
            public const string Attributes = "attributes";
            public const string ExcludedAttributes = "excludedAttributes";
            public const string Filter = "filter";
            public const string SortBy = "sortBy";
            public const string SortOrder = "sortOrder";
            public const string StartIndex = "startIndex";
            public const string Count = "count";
        }

        public static class SCIMEndpoints
        {
            public const string Users = "Users";
        }

        public static class StandardSchemas
        {
            public static ICollection<SCIMSchema> UserSchemas = new List<SCIMSchema>
            {
                 SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", "User Account")
                    .AddStringAttribute("userName", caseExact: true, uniqueness: SCIMSchemaAttributeUniqueness.SERVER)
                    .AddComplexAttribute("name", c =>
                    {
                        c.AddStringAttribute("formatted", description: "The full name");
                        c.AddStringAttribute("familyName", description: "The family name");
                        c.AddStringAttribute("givenName", description: "The given name");
                    }, description: "The components of the user's real name.")
                    .AddComplexAttribute("groups", opt => {
                        opt.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                    }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READONLY)
                    .Build()
            };
            public static SCIMSchema ErrorSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:Error", "Error", "SCIM errors")
                    .AddStringAttribute("status", required: true)
                    .AddStringAttribute("scimType")
                    .AddStringAttribute("detail")
                    .Build();
            public static SCIMSchema ListResponseSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:ListResponse", "Response", "List response")
                .Build();
        }
    }
}