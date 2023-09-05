// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using AspNetCore.Authentication.ApiKey;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Infrastructures;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using SimpleIdServer.Scim.Startup.Configurations;
using SimpleIdServer.Scim.Startup.Consumers;
using SimpleIdServer.Scim.Startup.Services;
using SimpleIdServer.Scim.SwashbuckleV6;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.Scim.Startup
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration) 
        {
            Env = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment Env { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var apiKeys = Configuration.GetSection(nameof(ApiKeysConfiguration)).Get<ApiKeysConfiguration>();
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
                o.AddSCIMValueProviders();
            }).AddNewtonsoftJson(o => { });
            services.AddSingleton(apiKeys);
            services.AddLogging(o =>
            {
                o.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });
            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
                {
                    options.Realm = "Sample Web API";
                    options.KeyName = "Authorization";
                });
            services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<EnumDocumentFilter>();
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                    .Union(new AssemblyName[] { currentAssembly.GetName() })
                    .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                    .Where(f => File.Exists(f)).ToArray();
                Array.ForEach(xmlDocs, (d) =>
                {
                    c.IncludeXmlComments(d);
                });
            });
            services.AddSCIMSwagger();
            services.AddSIDScim(_ =>
            {
                _.IsNoContentReturned = false;
                _.IgnoreUnsupportedCanonicalValues = false;
            }, massTransitOptions: _ =>
            {
                _.AddConsumer<IntegrationEventConsumer>();
                _.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
            ConfigureStorage(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
            });
            InitializeDatabase(app);
            app.UseAuthentication();
            app.UseMvc();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            var section = Configuration.GetSection(nameof(StorageConfiguration));
            var conf = section.Get<StorageConfiguration>();
            if (conf.Type == StorageTypes.MONGODB)
            {
                // MigrateFrom403To404MongoDB(app);
                return;
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<Persistence.EF.SCIMDbContext>())
                {
                    context.Database.Migrate();
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
                    if (!context.SCIMSchemaLst.Any())
                    {
                        context.SCIMSchemaLst.Add(userSchema);
                        context.SCIMSchemaLst.Add(groupSchema);
                        context.SCIMSchemaLst.Add(eidUserSchema);
                        context.SCIMSchemaLst.Add(entrepriseUser);
                    }

                    if (!context.SCIMAttributeMappingLst.Any())
                    {
                        var firstAttributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                            SourceResourceType = StandardSchemas.UserSchema.ResourceType,
                            SourceAttributeSelector = "groups",
                            TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
                            TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id
                        };
                        var secondAttributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                            SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                            SourceAttributeSelector = "members",
                            TargetResourceType = StandardSchemas.UserSchema.ResourceType,
                            TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                            Mode = Mode.PROPAGATE_INHERITANCE
                        };
                        var thirdAttributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                            SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                            SourceAttributeSelector = "members",
                            TargetResourceType = StandardSchemas.GroupSchema.ResourceType
                        };

                        context.SCIMAttributeMappingLst.Add(firstAttributeMapping);
                        context.SCIMAttributeMappingLst.Add(secondAttributeMapping);
                        context.SCIMAttributeMappingLst.Add(thirdAttributeMapping);
                    }

                    context.SaveChanges();
                }
            }

            // MigrateFrom403To404EF(app);
        }

        private static void MigrateFrom403To404EF(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<Persistence.EF.SCIMDbContext>())
                {
                    // Update IsComputedField
                    var targetAttributeIds = context.SCIMAttributeMappingLst.Where(a => a.TargetAttributeId != null).Select(a => a.TargetAttributeId);
                    var allIds = context.SCIMSchemaAttribute.Where(a => targetAttributeIds.Contains(a.ParentId) && (a.FullPath.EndsWith("display") || a.FullPath.EndsWith("type"))).Select(a => a.Id).ToList();
                    var filteredAttributes = context.SCIMRepresentationAttributeLst.Where(a => allIds.Contains(a.SchemaAttributeId));
                    foreach (var filteredAttribute in filteredAttributes)
                        filteredAttribute.IsComputed = true;
                    context.SaveChanges();

                    // Update ComputedIndex
                    var groupedAttributes = context.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).GroupBy(a => a.RepresentationId);
                    foreach(var grp in groupedAttributes)
                    {
                        SCIMRepresentation.BuildHierarchicalAttributes(grp).SelectMany(a => a.ToFlat());
                    }

                    context.SaveChanges();
                }
            }
        }

        private static async void MigrateFrom403To404MongoDB(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<Persistence.MongoDB.SCIMDbContext>())
                {
                    // Update IsComputedField
                    var targetAttributeIds = await context.SCIMAttributeMappingLst.AsQueryable().Where(a => a.TargetAttributeId != null).Select(a => a.TargetAttributeId).ToMongoListAsync();
                    var schemas = await context.SCIMSchemaLst.AsQueryable().ToMongoListAsync();
                    var allIds = schemas.SelectMany(s => s.Attributes).Where(a => targetAttributeIds.Contains(a.ParentId) && (a.FullPath.EndsWith("display") || a.FullPath.EndsWith("type"))).Select(a => a.Id);
                    var filteredAttributes = await context.SCIMRepresentationAttributeLst.AsQueryable().Where(a => allIds.Contains(a.SchemaAttributeId)).ToMongoListAsync();
                    foreach (var filteredAttribute in filteredAttributes)
                        filteredAttribute.IsComputed = true;
                    foreach (var attr in filteredAttributes)
                        await context.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr, new ReplaceOptions { IsUpsert = true });

                    // Update ComputedIndex
                    var groupedAttributes = context.SCIMRepresentationAttributeLst.AsQueryable().GroupBy(b => b.RepresentationId);
                    foreach (var grp in groupedAttributes)
                    {
                        var attrs = SCIMRepresentation.BuildHierarchicalAttributes(grp).SelectMany(a => a.ToFlat());
                        foreach (var attr in attrs)
                            await context.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr, new ReplaceOptions { IsUpsert = true });
                    }

                    // Update all the representations
                    var representations = await context.SCIMRepresentationLst.AsQueryable().ToMongoListAsync();
                    foreach(var representation in representations)
                    {
                        var filter = Builders<SCIMRepresentationModel>.Filter.Eq("_id", representation.Id);
                        var updateDefinitionBuilder = Builders<SCIMRepresentationModel>.Update;
                        var updateDefinition = updateDefinitionBuilder.Unset("FlatAttributes");
                        representation.AttributeRefs = representation.FlatAttributes.Select(a => new CustomMongoDBRef("representationAttributes", a.Id)).ToList();
                        representation.FlatAttributes = null;
                        await context.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == representation.Id, representation, new ReplaceOptions { IsUpsert = true });
                        await context.SCIMRepresentationLst.UpdateOneAsync(filter, updateDefinition);                    }
                }
            }
        }

        private void ConfigureStorage(IServiceCollection services)
        {
            var section = Configuration.GetSection(nameof(StorageConfiguration));
            var conf = section.Get<StorageConfiguration>();
            if (conf.Type == StorageTypes.MONGODB) ConfigureMongoDbStorage(services, conf);
            else ConfigureEFStorage(services, conf);
        }

        private void ConfigureMongoDbStorage(IServiceCollection services, StorageConfiguration conf)
        {
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
                opt.ConnectionString = conf.ConnectionString;
                opt.Database = "scim";
                opt.CollectionMappings = "mappings";
                opt.CollectionRepresentations = "representations";
                opt.CollectionSchemas = "schemas";
                opt.SupportTransaction = false;
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
            }, useVersion403: false);
        }

        private void ConfigureEFStorage(IServiceCollection services, StorageConfiguration conf)
        {
            services.AddScimStoreEF(options =>
            {
                switch (conf.Type)
                {
                    case StorageTypes.SQLSERVER:
                        options.UseSqlServer(conf.ConnectionString, o =>
                        {
                            o.MigrationsAssembly("SimpleIdServer.Scim.SqlServerMigrations");
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });
                        break;
                    case StorageTypes.POSTGRE:
                        options.UseNpgsql(conf.ConnectionString, o =>
                        {
                            o.MigrationsAssembly("SimpleIdServer.Scim.PostgreMigrations");
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });
                        break;
                }
            }, options =>
            {
                options.DefaultSchema = "scim";
            });
        }
    }
}
