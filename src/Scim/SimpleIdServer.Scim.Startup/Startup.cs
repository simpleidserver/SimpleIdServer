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
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Startup.Consumers;
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
            var basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Schemas");
            var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMConstants.SCIMEndpoints.User, true);
            var enterpriseUserSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EnterpriseUserSchema.json"), SCIMConstants.SCIMEndpoints.User);
            var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMConstants.SCIMEndpoints.Group, true);
            var customResource = Builder.SCIMSchemaBuilder.Create("urn:customresource", "CustomResources", "CustomResources", string.Empty, true)
                .AddStringAttribute("name")
                .AddStringAttribute("lastname")
                .AddDateTimeAttribute("birthDate")
                .Build();
            var entitlementSchema = Builder.SCIMSchemaBuilder.Create("urn:entitlement", "Entitlement", "Entitlements", string.Empty, true)
                .AddStringAttribute("displayName")
                .AddComplexAttribute("members", opt =>
                {
                    opt.AddStringAttribute("value");
                    opt.AddStringAttribute("$ref");
                    opt.AddStringAttribute("type");
                }, multiValued: true)
                .Build();
            var customUserSchema = Builder.SCIMSchemaBuilder.Create("urn:customuser", "CustomUser", "CustomUsers", string.Empty, true)
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
            services.AddMassTransitHostedService(true);
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
                    SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                    SourceValueAttributeId = userSchema.Attributes.First(a => a.Name == "groups").SubAttributes.First(g => g.Name == "value").Id,
                    SourceResourceType = SCIMConstants.StandardSchemas.UserSchema.ResourceType,
                    SourceAttributeSelector = "groups",
                    TargetResourceType = SCIMConstants.StandardSchemas.GroupSchema.ResourceType,
                    TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(a => a.Name == "value").Id
                },
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceAttributeId = customUserSchema.Attributes.First(a => a.Name == "entitlements").Id,
                    SourceValueAttributeId = customUserSchema.Attributes.First(a => a.Name == "entitlements").SubAttributes.First(g => g.Name == "value").Id,
                    SourceResourceType = "CustomUsers",
                    SourceAttributeSelector = "entitlements",
                    TargetResourceType = "Entitlements",
                    TargetAttributeId = entitlementSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(a => a.Name == "value").Id
                },
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceAttributeId = entitlementSchema.Attributes.First(a => a.Name == "members").Id,
                    SourceValueAttributeId = entitlementSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(g => g.Name == "value").Id,
                    SourceResourceType = "Entitlements",
                    SourceAttributeSelector = "members",
                    TargetResourceType = "CustomUsers",
                    TargetAttributeId = customUserSchema.Attributes.First(a => a.Name == "entitlements").SubAttributes.First(a => a.Name == "value").Id
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