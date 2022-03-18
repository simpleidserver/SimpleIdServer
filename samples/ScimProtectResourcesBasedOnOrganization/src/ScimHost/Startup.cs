// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace ScimHost
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IWebHostEnvironment  webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
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
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("QueryScimResource", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("AddScimResource", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("DeleteScimResource", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("UpdateScimResource", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("BulkScimResource", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("UserAuthenticated", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("Provison", p => p.RequireClaim("scope", "admin"));
                opts.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
            });
            services.AddAuthentication(SCIMConstants.AuthenticationScheme)
                .AddJwtBearer(SCIMConstants.AuthenticationScheme, cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "http://localhost:5001",
                        ValidAudiences = new List<string>
                        {
                            "admin", "firstOrg", "secondOrg"
                        },
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            var basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Schemas");
            var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
            var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMResourceTypes.Group, true);
            var enterpriseUserSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EnterpriseUserSchema.json"), SCIMResourceTypes.User);
            userSchema.SchemaExtensions.Add(new SCIMSchemaExtension
            {
                Id = Guid.NewGuid().ToString(),
                Schema = enterpriseUserSchema.Id
            });
            var schemas = new List<SCIMSchema>
            {
                userSchema,
                groupSchema,
                enterpriseUserSchema
            };
            services.AddSwaggerGen(c =>
            {
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
            services.AddSIDScim(options: _ =>
            {
                _.IgnoreUnsupportedCanonicalValues = false;
            })
            .AddSchemas(schemas)
            .AddAttributeMapping(new List<SCIMAttributeMapping>
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
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
            });
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMvc();
        }
    }
}