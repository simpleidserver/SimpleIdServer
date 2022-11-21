// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Domains.Builders;
using SimpleIdServer.Scim.Startup.Consumers;
using SimpleIdServer.Scim.Swashbuckle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace SimpleIdServer.Scim.Startup
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
            services.AddLogging(opt =>
            {
                opt.AddConsole();
                opt.AddFilter((s, l) =>
                {
                    return s.StartsWith("SimpleIdServer.Scim");
                });
            });
            services.AddAuthorization(opts =>
            {
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
                        ValidIssuer = "https://localhost:60000",
                        ValidAudiences = new List<string>
                        {
                            "scimClient", "gatewayClient"
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            var basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Schemas");
            var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
            var enterpriseUserSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EnterpriseUserSchema.json"), SCIMResourceTypes.User);
            var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMResourceTypes.Group, true);
            var customResource = SCIMSchemaBuilder.Create("urn:customresource", "CustomResource", "CustomResource", string.Empty, true)
                .AddStringAttribute("name")
                .AddStringAttribute("lastname")
                .AddDateTimeAttribute("birthDate")
                .Build();
            var entitlementSchema = SCIMSchemaBuilder.Create("urn:entitlement", "Entitlement", "Entitlement", string.Empty, true)
                .AddStringAttribute("displayName")
                .AddComplexAttribute("members", opt =>
                {
                    opt.AddStringAttribute("value");
                    opt.AddStringAttribute("$ref");
                    opt.AddStringAttribute("type");
                }, multiValued: true)
                .Build();
            var customUserSchema = SCIMSchemaBuilder.Create("urn:customuser", "CustomUser", "CustomUser", string.Empty, true)
                .AddStringAttribute("userName", required: true)
                .AddComplexAttribute("entitlements", opt =>
                {
                    opt.AddStringAttribute("value");
                    opt.AddStringAttribute("$ref");
                    opt.AddStringAttribute("type");
                }, multiValued: true)
                .Build();
            userSchema.SchemaExtensions.Add(new SCIMSchemaExtension
            {
                Id = Guid.NewGuid().ToString(),
                Schema = enterpriseUserSchema.Id
            });
            var schemas = new List<SCIMSchema>
            {
                userSchema,
                groupSchema,
                enterpriseUserSchema,
                customResource,
                entitlementSchema,
                customUserSchema
            };
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
            services.AddSIDScim(options: _ =>
            {
                _.IgnoreUnsupportedCanonicalValues = false;
            }, massTransitOptions: _ =>
            {
                _.AddConsumer<IntegrationEventConsumer>();
                _.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .AddSchemas(schemas)
            .AddAttributeMapping(new List<SCIMAttributeMapping>
            {
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                    SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                    SourceAttributeSelector = "members",
                    TargetResourceType = StandardSchemas.UserSchema.ResourceType,
                    TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id
                },
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                    SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                    SourceAttributeSelector = "members",
                    TargetResourceType = StandardSchemas.GroupSchema.ResourceType
                },
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
                    SourceAttributeId = customUserSchema.Attributes.First(a => a.Name == "entitlements").Id,
                    SourceResourceType = "CustomUser",
                    SourceAttributeSelector = "entitlements",
                    TargetResourceType = "Entitlement",
                    TargetAttributeId = entitlementSchema.Attributes.First(a => a.Name == "members").Id
                },
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceAttributeId = entitlementSchema.Attributes.First(a => a.Name == "members").Id,
                    SourceResourceType = "Entitlement",
                    SourceAttributeSelector = "members",
                    TargetResourceType = "CustomUser",
                    TargetAttributeId = customUserSchema.Attributes.First(a => a.Name == "entitlements").Id
                }
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