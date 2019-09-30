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
        }

        public static class StandardSCIMMetaAttributes
        {
            public const string ResourceType = "resourceType";
            public const string Created = "created";
            public const string LastModified = "lastModified";
            public const string Location = "location";
            public const string Version = "version";
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
                        c.AddStringAttribute("formatted");
                        c.AddStringAttribute("familyName");
                        c.AddStringAttribute("givenName");
                    }, mutability: SCIMSchemaAttributeMutabilities.WRITEONLY)
                    .Build()
            };
            public static SCIMSchema ErrorSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:Error", "Error", "SCIM errors")
                    .AddStringAttribute("status", required: true)
                    .AddStringAttribute("scimType")
                    .AddStringAttribute("detail")
                    .Build();
        }
    }
}