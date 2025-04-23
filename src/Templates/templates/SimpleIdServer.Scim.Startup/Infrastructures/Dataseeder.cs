// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB;
using SimpleIdServer.Scim.Startup.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleIdServer.Scim.Startup.Infrastructures;

public class Dataseeder
{
    public static void Seed(WebApplicationBuilder builder, WebApplication app, StorageConfiguration configuration)
    {
        var basePath = Path.Combine(builder.Environment.ContentRootPath, "Schemas");
        var userSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
        var eidUserSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EIDUserSchema.json"), SCIMResourceTypes.User);
        var groupSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMResourceTypes.Group, true);
        var entrepriseUser = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EnterpriseUser.json"), SCIMResourceTypes.User);
        var schemas = new List<SCIMSchema>
        {
            userSchema,
            eidUserSchema,
            groupSchema,
            entrepriseUser
        };
        userSchema.SchemaExtensions.Add(new SCIMSchemaExtension
        {
            Id = Guid.NewGuid().ToString(),
            Schema = "urn:ietf:params:scim:schemas:extension:eid:2.0:User"
        });
        userSchema.SchemaExtensions.Add(new SCIMSchemaExtension
        {
            Id = Guid.NewGuid().ToString(),
            Schema = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User"
        });
        var attributes = new List<SCIMAttributeMapping>
        {
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                SourceResourceType = StandardSchemas.UserSchema.ResourceType,
                SourceAttributeSelector = "groups",
                TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
                TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id
            },
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                SourceAttributeSelector = "members",
                TargetResourceType = StandardSchemas.UserSchema.ResourceType,
                TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                Mode = Mode.PROPAGATE_INHERITANCE
            },
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                SourceAttributeSelector = "members",
                TargetResourceType = StandardSchemas.GroupSchema.ResourceType
            }
        };
        var realms = SimpleIdServer.Scim.SCIMConstants.StandardRealms;
        if(configuration.Type == StorageTypes.MONGODB)
        {
            app.Services.EnsureMongoStoreDataMigrated(schemas, attributes, realms);
        }
        else
        {
            app.Services.EnsureEfStoreMigrated(schemas, attributes, realms);
        }
    }
}
