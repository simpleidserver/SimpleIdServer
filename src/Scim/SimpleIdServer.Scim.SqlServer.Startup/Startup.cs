// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace SimpleIdServer.Scim.SqlServer.Startup
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration configuration) 
        {
            _webHostEnvironment = webHostEnvironment;
            Configuration = configuration;
        }

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
            services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
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
            });
            services.AddScimStoreEF(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("db"), o => o.MigrationsAssembly(migrationsAssembly)).EnableSensitiveDataLogging();
            });
            services.AddDistributedLockSQLServer(opts =>
            {
                opts.ConnectionString = Configuration.GetConnectionString("db");
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
                    var basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Schemas");
                    var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMConstants.SCIMEndpoints.User);
                    var eidUserSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EIDUserSchema.json"), SCIMConstants.SCIMEndpoints.User);
                    var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMConstants.SCIMEndpoints.Group);
                    userSchema.SchemaExtensions.Add(new SCIMSchemaExtension
                    {
                        Id = Guid.NewGuid().ToString(),
                        Schema = "urn:ietf:params:scim:schemas:extension:eid:2.0:User"
                    });
                    if (!context.SCIMSchemaLst.Any())
                    {
                        context.SCIMSchemaLst.Add(userSchema.ToModel());
                        context.SCIMSchemaLst.Add(groupSchema.ToModel());
                        context.SCIMSchemaLst.Add(eidUserSchema.ToModel());
                    }

                    if (!context.SCIMAttributeMappingLst.Any())
                    {
                        var attributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceResourceType = SCIMConstants.StandardSchemas.UserSchema.ResourceType,
                            SourceAttributeSelector = "groups",
                            TargetResourceType = SCIMConstants.StandardSchemas.GroupSchema.ResourceType,
                            TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(a => a.Name == "value").Id
                        };
                        context.SCIMAttributeMappingLst.Add(attributeMapping.ToModel());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}