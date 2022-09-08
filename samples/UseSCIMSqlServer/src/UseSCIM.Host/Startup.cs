// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UseSCIM.Host.Consumers;

namespace UseSCIM.Host
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration) 
        {
            Env = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment Env { get; }
        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var json = File.ReadAllText("oauth_puk.txt");
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var rsaParameters = new RSAParameters
            {
                Modulus = dic.TryGet(RSAFields.Modulus),
                Exponent = dic.TryGet(RSAFields.Exponent)
            };
            var oauthRsaSecurityKey = new RsaSecurityKey(rsaParameters);
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
                o.AddSCIMValueProviders();
            }).AddNewtonsoftJson(o => { });
            services.AddLogging();
            services.AddAuthorization(opts =>
            {
                // By pass authorization rules.
                opts.AddPolicy("QueryScimResource", p => p.RequireAssertion(_ => true));
                opts.AddPolicy("AddScimResource", p => p.RequireAssertion(_ => true));
                opts.AddPolicy("DeleteScimResource", p => p.RequireAssertion(_ => true));
                opts.AddPolicy("UpdateScimResource", p => p.RequireAssertion(_ => true));
                opts.AddPolicy("BulkScimResource", p => p.RequireAssertion(_ => true));
                opts.AddPolicy("UserAuthenticated", p => p.RequireAssertion(_ => true));
                opts.AddPolicy("Provison", p => p.RequireAssertion(_ => true));
            });
            services.AddAuthentication(SCIMConstants.AuthenticationScheme)
                .AddJwtBearer(SCIMConstants.AuthenticationScheme, cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "http://localhost:60000",
                        ValidAudiences = new List<string>
                        {
                            "scimClient"
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            services.AddSIDScim(_ =>
            {
                _.IgnoreUnsupportedCanonicalValues = false;
            }, massTransitOptions: _ =>
            {
                _.AddConsumer<IntegrationEventConsumer>();
                _.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddScimStoreEF(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("db"), o => o.MigrationsAssembly(migrationsAssembly));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            app.UseAuthentication();
            app.UseMvc();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SCIMDbContext>())
                {
                    context.Database.Migrate();
                    var basePath = Path.Combine(Env.ContentRootPath, "Schemas");
                    var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
                    var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMResourceTypes.Group, true);
                    if (!context.SCIMSchemaLst.Any())
                    {
                        context.SCIMSchemaLst.Add(userSchema);
                        context.SCIMSchemaLst.Add(groupSchema);
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
                            TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id
                        };
                        context.SCIMAttributeMappingLst.Add(firstAttributeMapping);
                        context.SCIMAttributeMappingLst.Add(secondAttributeMapping);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}