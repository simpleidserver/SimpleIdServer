// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using AspNetCore.Authentication.ApiKey;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;
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

namespace SimpleIdServer.Scim.Startup;

public class Program
{
    private const bool usePrefix = true;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        ConfigureServices(builder);
        var app = builder.Build();
        ConfigureApp(builder, app);
        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddMvc(o =>
        {
            o.EnableEndpointRouting = false;
            o.AddSCIMValueProviders();
        }).AddNewtonsoftJson();
        builder.Services.AddLogging(o =>
        {
            o.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        });
        ConfigureAuthorization(builder);
        ConfigureSwagger(builder);
        ConfigureScim(builder);
        builder.Services.Configure<RouteOptions>(opt =>
        {
            opt.ConstraintMap.Add("realmPrefix", typeof(RealmRoutePrefixConstraint));
        });
    }

    #region Dependency injection

    private static void ConfigureAuthorization(WebApplicationBuilder builder)
    {
        var apiKeys = builder.Configuration.GetSection(nameof(ApiKeysConfiguration)).Get<ApiKeysConfiguration>();
        builder.Services.AddSingleton(apiKeys);
        builder.Services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
            .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
            {
                options.Realm = "Sample Web API";
                options.KeyName = "Authorization";
            });
        builder.Services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
        /*
        builder.Services.AddAuthorization(opts =>
        {
            opts.AddPolicy("QueryScimResource", p => p.RequireAssertion(_ => true));
            opts.AddPolicy("AddScimResource", p => p.RequireAssertion(_ => true));
            opts.AddPolicy("DeleteScimResource", p => p.RequireAssertion(_ => true));
            opts.AddPolicy("UpdateScimResource", p => p.RequireAssertion(_ => true));
            opts.AddPolicy("BulkScimResource", p => p.RequireAssertion(_ => true));
        });
        */
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
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
        builder.Services.AddSCIMSwagger();
    }

    private static void ConfigureScim(WebApplicationBuilder builder)
    {
        builder.Services.AddSIDScim(_ =>
        {
            _.IgnoreUnsupportedCanonicalValues = false;
            _.EnableRealm = bool.Parse(builder.Configuration["IsRealmEnabled"]);
        }, massTransitOptions: _ =>
        {
            _.AddConsumer<IntegrationEventConsumer>();
            _.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
        var conf = section.Get<StorageConfiguration>();
        if (conf.Type == StorageTypes.MONGODB) ConfigureMongoDbStorage(conf);
        else ConfigureEFStorage(conf);

        void ConfigureEFStorage(StorageConfiguration conf)
        {
            builder.Services.AddScimStoreEF(options =>
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
                    case StorageTypes.MYSQL:
                        options.UseMySql(conf.ConnectionString, ServerVersion.AutoDetect(conf.ConnectionString), o =>
                        {
                            o.MigrationsAssembly("SimpleIdServer.Scim.MySQLMigrations");
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            o.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
                        });
                        break;
                    case StorageTypes.SQLITE:
                        options.UseSqlite(conf.ConnectionString, o =>
                        {
                            o.MigrationsAssembly("SimpleIdServer.Scim.SqliteMigrations");
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });
                        break;
                }
            }, options =>
            {
                options.DefaultSchema = "scim";
            }, supportSqlite: conf.Type == StorageTypes.SQLITE);
        }

        void ConfigureMongoDbStorage(StorageConfiguration conf)
        {
            var basePath = Path.Combine(builder.Environment.ContentRootPath, "Schemas");
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
            builder.Services.AddScimStoreMongoDB(opt =>
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
    }

    #endregion

    private static void ConfigureApp(WebApplicationBuilder builder, WebApplication app)
    {
        var opts = app.Services.GetRequiredService<IOptions<SimpleIdServer.Scim.SCIMHostOptions>>().Value;
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
        });
        InitializeDatabase(builder, app);
        app.UseAuthentication();
        app.UseMvc(o =>
        {
            o.UseScim(opts.EnableRealm);
        });
    }

    #region Database migration

    private static void InitializeDatabase(WebApplicationBuilder builder, IApplicationBuilder app)
    {
        var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
        var conf = section.Get<StorageConfiguration>();
        if (conf.Type == StorageTypes.MONGODB)
        {
            // MigrateFrom403To404MongoDB(app);
            return;
        }

        using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<SimpleIdServer.Scim.Persistence.EF.SCIMDbContext>())
            {
                context.Database.Migrate();
                var basePath = Path.Combine(builder.Environment.ContentRootPath, "Schemas");
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

                if (!context.Realms.Any())
                    context.Realms.AddRange(SimpleIdServer.Scim.SCIMConstants.StandardRealms);

                context.SaveChanges();
            }
        }

        // MigrateFrom403To404EF(app);
    }

    private static void MigrateFrom403To404EF(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<SimpleIdServer.Scim.Persistence.EF.SCIMDbContext>())
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
                foreach (var grp in groupedAttributes)
                {
                    SCIMRepresentation.BuildHierarchicalAttributes(grp).SelectMany(a => a.ToFlat());
                }

                context.SaveChanges();
            }
        }
    }

    private static void MigrateFrom403To404MongoDB(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            using (var context = scope.ServiceProvider.GetService<SimpleIdServer.Scim.Persistence.MongoDB.SCIMDbContext>())
            {
                // Update IsComputedField
                var targetAttributeIds = context.SCIMAttributeMappingLst.AsQueryable().Where(a => a.TargetAttributeId != null).Select(a => a.TargetAttributeId).ToMongoListAsync().Result;
                var schemas = context.SCIMSchemaLst.AsQueryable().ToMongoListAsync().Result;
                var allIds = schemas.SelectMany(s => s.Attributes).Where(a => targetAttributeIds.Contains(a.ParentId) && (a.FullPath.EndsWith("display") || a.FullPath.EndsWith("type"))).Select(a => a.Id);
                var filteredAttributes = context.SCIMRepresentationAttributeLst.AsQueryable().Where(a => allIds.Contains(a.SchemaAttributeId)).ToMongoListAsync().Result;
                foreach (var filteredAttribute in filteredAttributes)
                    filteredAttribute.IsComputed = true;
                foreach (var attr in filteredAttributes)
                    context.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr, new ReplaceOptions { IsUpsert = true }).Wait();

                // Update ComputedIndex
                var groupedAttributes = context.SCIMRepresentationAttributeLst.AsQueryable().GroupBy(b => b.RepresentationId);
                foreach (var grp in groupedAttributes)
                {
                    var attrs = SCIMRepresentation.BuildHierarchicalAttributes(grp).SelectMany(a => a.ToFlat());
                    foreach (var attr in attrs)
                        context.SCIMRepresentationAttributeLst.ReplaceOneAsync(s => s.Id == attr.Id, attr, new ReplaceOptions { IsUpsert = true }).Wait();
                }

                // Update all the representations
                var representations = context.SCIMRepresentationLst.AsQueryable().ToMongoListAsync().Result;
                foreach (var representation in representations)
                {
                    var filter = Builders<SCIMRepresentationModel>.Filter.Eq("_id", representation.Id);
                    var updateDefinitionBuilder = Builders<SCIMRepresentationModel>.Update;
                    var updateDefinition = updateDefinitionBuilder.Unset("FlatAttributes");
                    representation.AttributeRefs = representation.FlatAttributes.Select(a => new CustomMongoDBRef("representationAttributes", a.Id)).ToList();
                    representation.FlatAttributes = null;
                    context.SCIMRepresentationLst.ReplaceOneAsync(s => s.Id == representation.Id, representation, new ReplaceOptions { IsUpsert = true }).Wait();
                    context.SCIMRepresentationLst.UpdateOneAsync(filter, updateDefinition).Wait();
                }
            }
        }
    }

    #endregion
}