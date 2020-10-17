// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim
{
    public static class SCIMConstants
    {
        public const string AuthenticationScheme = "SimpleIdServerSCIM";

        public const string STANDARD_SCIM_CONTENT_TYPE = "application/scim+json";

        public static class ErrorSCIMTypes
        {
            public const string InvalidSyntax = "invalidSyntax";
            public const string Uniqueness = "uniqueness";
            public const string InternalServerError = "internalServerError";
            public const string Unknown = "unknown";
            public const string Mutability = "mutability";
            public const string InvalidFilter = "invalidFilter";
            public const string SchemaViolated = "schemaViolated";
            public const string TooLarge = "tooLarge";
        }

        public static ICollection<string> StandardSCIMCommonRepresentationAttributes = new List<string>
        {
            StandardSCIMRepresentationAttributes.Id,
            StandardSCIMRepresentationAttributes.ExternalId,
            StandardSCIMRepresentationAttributes.Meta
        };

        public static class StandardSCIMRepresentationAttributes
        {
            public const string Schemas = "schemas";
            public const string Meta = "meta";
            public const string Id = "id";
            public const string Name = "name";
            public const string Description = "description";
            public const string Attributes = "attributes";
            public const string ExternalId = "externalId";
            public const string TotalResults = "totalResults";
            public const string StartIndex = "startIndex";
            public const string ItemsPerPage = "itemsPerPage";
            public const string Resources = "Resources";
            public const string Operations = "Operations";
            public const string Method = "method";
            public const string Path = "path";
            public const string BulkId = "bulkId";
            public const string Data = "data";
            public const string Location = "location";
            public const string Version = "version";
            public const string Type = "type";
            public const string MultiValued = "multiValued";
            public const string Required = "required";
            public const string CaseExact = "caseExact";
            public const string Mutability = "mutability";
            public const string Returned = "returned";
            public const string Uniqueness = "uniqueness";
            public const string SubAttributes = "subAttributes";
            public const string CanonicalValues = "canonicalValues";
        }

        public static class PathOperationAttributes
        {
            public const string Operation = "op";
            public const string Path = "path";
            public const string Value = "value";
            public const string Operations = "Operations";
        }

        public static class ResourceTypeAttribute
        {
            public const string Schemas = "schemas";
            public const string Id = "id";
            public const string Name = "name";
            public const string Description = "description";
            public const string Endpoint = "endpoint";
            public const string SchemaExtensions = "schemaExtensions";
            public const string Schema = "schema";
            public const string Required = "required";
            public const string Meta = "meta";
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
            public const string User = "Users";
            public const string Group = "Groups";
            public const string ServiceProviderConfig = "ServiceProviderConfig";
            public const string Bulk = "Bulk";
            public const string Schemas = "Schemas";
            public const string ResourceType = "ResourceTypes";
        }

        public static List<SCIMAttributeMapping> StandardAttributeMapping = new List<SCIMAttributeMapping>
        {
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceResourceType = StandardSchemas.UserSchema.ResourceType,
                SourceAttributeSelector = "groups",
                TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
                TargetAttributeId = StandardSchemas.GroupSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(a => a.Name == "value").Id
            }
        };

        public static class StandardSchemas
        {
            public static SCIMSchema ResourceTypeSchema =
                SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:ResourceType", "ResourceType", SCIMEndpoints.ResourceType, "Resource type", true)
                    .AddStringAttribute(ResourceTypeAttribute.Id)
                    .AddStringAttribute(ResourceTypeAttribute.Name, required: true)
                    .AddStringAttribute(ResourceTypeAttribute.Description)
                    .AddStringAttribute(ResourceTypeAttribute.Endpoint, required: true)
                    .AddStringAttribute(ResourceTypeAttribute.Schema, required: true)
                    .AddComplexAttribute(ResourceTypeAttribute.SchemaExtensions, callback: c =>
                    {
                        c.AddStringAttribute(ResourceTypeAttribute.Schema, required: true);
                        c.AddStringAttribute(ResourceTypeAttribute.Required, required: true);
                    }).Build();
            public static SCIMSchema UserSchema =
                 SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", SCIMEndpoints.User, "User Account", true)
                    .AddStringAttribute("userName", caseExact: true, uniqueness: SCIMSchemaAttributeUniqueness.SERVER)
                    .AddComplexAttribute("name", c =>
                    {
                        c.AddStringAttribute("formatted", description: "The full name");
                        c.AddStringAttribute("familyName", description: "The family name");
                        c.AddStringAttribute("givenName", description: "The given name");
                        c.AddStringAttribute("middleName", description: "The middle name");
                        c.AddStringAttribute("honorificPrefix");
                        c.AddStringAttribute("honorificSuffix");
                    }, description: "The components of the user's real name.")
                    .AddStringAttribute("displayName")
                    .AddStringAttribute("nickName")
                    .AddStringAttribute("profileUrl")
                    .AddStringAttribute("title")
                    .AddStringAttribute("userType")
                    .AddStringAttribute("preferredLanguage")
                    .AddStringAttribute("locale")
                    .AddStringAttribute("timezone")
                    .AddBooleanAttribute("active")
                    .AddStringAttribute("password")
                    .AddComplexAttribute("emails", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");
                    }, multiValued: true)
                    .AddComplexAttribute("phoneNumbers", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");

                    }, multiValued: true)
                    .AddComplexAttribute("ims", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");
                    }, multiValued: true)
                    .AddComplexAttribute("photos", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");
                    }, multiValued: true)
                    .AddComplexAttribute("addresses", callback: c =>
                    {
                        c.AddStringAttribute("formatted");
                        c.AddStringAttribute("streetAddress");
                        c.AddStringAttribute("locality");
                        c.AddStringAttribute("region");
                        c.AddStringAttribute("postalCode");
                        c.AddStringAttribute("country");
                        c.AddStringAttribute("type");
                    }, multiValued: true)
                    .AddComplexAttribute("groups", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("$ref");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                    }, multiValued: true)
                    .AddComplexAttribute("entitlements", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");
                    }, multiValued: true)
                    .AddComplexAttribute("roles", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");
                    }, multiValued: true)
                    .AddComplexAttribute("x509Certificates", callback: c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("display");
                        c.AddStringAttribute("type");
                        c.AddBooleanAttribute("primary");
                    }, multiValued: true)
                    .AddComplexAttribute("groups", opt =>
                    {
                        opt.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                    }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READONLY)
                    .Build();

            public static SCIMSchema GroupSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:Group", "Group", SCIMEndpoints.Group, "Group", true)
                    .AddStringAttribute("displayName")
                    .AddComplexAttribute("members", c =>
                    {
                        c.AddStringAttribute("value");
                        c.AddStringAttribute("$ref");
                        c.AddStringAttribute("display");
                    }, multiValued: true)
                    .Build();
            public static SCIMSchema ErrorSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:Error", "Error", null, "SCIM errors", true)
                    .AddStringAttribute("status", required: true)
                    .AddStringAttribute("scimType")
                    .AddStringAttribute("detail")
                    .Build();
            public static SCIMSchema ListResponseSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:ListResponse", "SearchResponse", null, "List response", true)
                .Build();
            public static SCIMSchema PatchRequestSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:PatchOp", "Patch", null, "Patch representation")
                .Build();
            public static SCIMSchema ServiceProvideConfigSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig", "Service Provider Configuration", null, "Schema for representing the service provider's configuration", true)
                .AddStringAttribute("documentationUrl")
                .AddComplexAttribute("patch", callback: c =>
                {
                    c.AddBooleanAttribute("supported");
                })
                .AddComplexAttribute("bulk", callback: c =>
                {
                    c.AddBooleanAttribute("supported");
                    c.AddIntAttribute("maxOperations", description: "An integer value specifying the maximum number of operations.");
                    c.AddIntAttribute("maxPayloadSize", description: "An integer value specifying the maximum payload size in bytes.");
                })
                .AddComplexAttribute("filter", callback: c =>
                {
                    c.AddBooleanAttribute("supported");
                    c.AddIntAttribute("maxResults", description: "An integer value specifying the maximum number of resources returned in a response.");
                })
                .AddComplexAttribute("changePassword", callback: c =>
                {
                    c.AddBooleanAttribute("supported");
                })
                .AddComplexAttribute("sort", callback: c =>
                {
                    c.AddBooleanAttribute("supported");
                })
                .AddComplexAttribute("etag", callback: c =>
                {
                    c.AddBooleanAttribute("supported");
                })
                .AddComplexAttribute("authenticationSchemes", callback: c =>
                {
                    c.AddStringAttribute("name");
                    c.AddStringAttribute("description");
                    c.AddStringAttribute("specUri");
                    c.AddStringAttribute("documentationUri");
                    c.AddStringAttribute("type");
                    c.AddBooleanAttribute("primary");
                }, multiValued: true)
                .Build();
            public static SCIMSchema BulkRequestSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:BulkRequest", "BulkRequest", null, "BulkRequest", true)
                .AddComplexAttribute(StandardSCIMRepresentationAttributes.Operations, callback: c =>
                {
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.Method);
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.Path);
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.BulkId);
                }, multiValued: true)
                .Build();
            public static SCIMSchema BulkResponseSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:BulkResponse", "BulkResponse", null, "BulkResponse", true)
                .AddComplexAttribute(StandardSCIMRepresentationAttributes.Operations, callback: c =>
                {
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.Location);
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.Method);
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.BulkId);
                    c.AddStringAttribute(StandardSCIMRepresentationAttributes.Version);
                }, multiValued: true)
                .Build();
        }
    }
}