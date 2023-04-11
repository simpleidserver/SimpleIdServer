// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.MongoDb.Startup.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleIdServer.Scim.MongoDb.Startup
{
    public class Startup
    {
        private const string DATABASE_NAME = "scim";
        public const string REPRESENTATIONS = "representations";
        public const string SCHEMAS = "schemas";
        public const string MAPPINGS = "mappings";
        public const bool SUPPORT_TRANSACTION = false;

        public Startup(IWebHostEnvironment env, IConfiguration configuration) 
        {
            Env = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment Env { get; }
        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
                o.AddSCIMValueProviders();
            }).AddNewtonsoftJson(o => { });
            services.AddLogging();
            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
                {
                    options.Realm = "Sample Web API";
                    options.KeyName = "Authorization";
                });
            services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
            services.AddSIDScim(_ =>
            {
                _.IgnoreUnsupportedCanonicalValues = false;
            });
            var basePath = Path.Combine(Env.ContentRootPath, "Schemas");
            var userSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
            var eidUserSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EIDUserSchema.json"), SCIMResourceTypes.User);
            var groupSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMResourceTypes.Group, true);
            var entrepriseUser = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EnterpriseUser.json"), SCIMResourceTypes.User);
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
            var schemas = new List<SCIMSchema>
            {
                userSchema,
                groupSchema,
                eidUserSchema,
                entrepriseUser
            };
            services.AddScimStoreMongoDB(opt =>
            {
                opt.ConnectionString = Configuration.GetConnectionString("db");
                opt.Database = DATABASE_NAME;
                opt.CollectionMappings = MAPPINGS;
                opt.CollectionRepresentations = REPRESENTATIONS;
                opt.CollectionSchemas = SCHEMAS;
                opt.SupportTransaction = SUPPORT_TRANSACTION;
            }, schemas,
            new List<SCIMAttributeMapping>
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
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMvc();
        }

    }
}