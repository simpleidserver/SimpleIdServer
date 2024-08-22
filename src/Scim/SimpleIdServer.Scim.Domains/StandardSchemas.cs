// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains.Builders;

namespace SimpleIdServer.Scim.Domains
{

    public static class StandardSchemas
    {
        public static SCIMSchema ResourceTypeSchema =
            SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:ResourceType", "ResourceType", SCIMResourceTypes.ResourceType, "Resource type", true)
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
             SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", SCIMResourceTypes.User, "User Account", true)
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
                    c.AddBooleanAttribute("primary");
                }, multiValued: true)
                .AddComplexAttribute("groups", callback: c =>
                {
                    c.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                    c.AddStringAttribute("$ref", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                    c.AddStringAttribute("display", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                    c.AddStringAttribute("type", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READONLY)
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
                //.AddComplexAttribute("groups", opt =>
                //{
                //    opt.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.READONLY);
                //}, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READONLY)
                .Build();

        public static SCIMSchema GroupSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:Group", "Group", SCIMResourceTypes.Group, "Group", true)
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
        public static SCIMSchema StandardResponseSchemas = SCIMSchemaBuilder.Create("urn:ietf:params:scim:api:messages:2.0:StandardResponse", "StandardResponse", null, "StandardResponse", true)
            .AddUniqueIdAttribute("A unique identifier for a SCIM resource as defined by the service provider")
            .AddStringAttribute(StandardSCIMRepresentationAttributes.ExternalId, description: " A String that is an identifier for the resource as defined by the provisioning client.")
            .AddStringAttribute(StandardSCIMRepresentationAttributes.Schemas, multiValued: true, description: " A multi valued String specifying schemas." )
            .AddComplexAttribute(StandardSCIMRepresentationAttributes.Meta, callback: c =>
            {
                c.AddStringAttribute(StandardSCIMMetaAttributes.ResourceType, description: "The name of the resource type of the resource.");
                c.AddDateTimeAttribute(StandardSCIMMetaAttributes.Created, description: "The DateTime that the resource was added to the service provider");
                c.AddDateTimeAttribute(StandardSCIMMetaAttributes.LastModified, description: "The most recent DateTime that the details of this resource were updated at the service provider");
                c.AddIntAttribute(StandardSCIMMetaAttributes.Version, description: "The version of the resource being returned");
                c.AddStringAttribute(StandardSCIMMetaAttributes.Location, description: "The URI of the resource being returned");
            }, multiValued: false, description: "A complex attribute containing resource metadata")
            .Build();
    }
}
