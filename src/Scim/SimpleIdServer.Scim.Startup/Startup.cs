// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleIdServer.Scim.Startup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Env = env;
        }

        private IHostingEnvironment Env { get; }

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
            services.AddMvc();
            services.AddLogging(opt =>
            {
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
            var basePath = Path.Combine(Env.ContentRootPath, "Schemas");
            var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMConstants.SCIMEndpoints.User);
            var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMConstants.SCIMEndpoints.Group);
            var customResource = Builder.SCIMSchemaBuilder.Create("urn:customresource", "CustomResources", "CustomResources")
                .AddStringAttribute("name")
                .AddStringAttribute("lastname")
                .Build();
            var schemas = new List<SCIMSchema>
            {
                userSchema,
                groupSchema,
                customResource
            };
            services.AddSIDScim(_ =>
            {
                _.IgnoreUnsupportedCanonicalValues = false;
            })
            .AddSchemas(schemas)
            .AddAttributeMapping(new List<SCIMAttributeMapping>
            {
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceResourceType = SCIMConstants.StandardSchemas.UserSchema.ResourceType,
                    SourceAttributeSelector = "groups",
                    TargetResourceType = SCIMConstants.StandardSchemas.GroupSchema.ResourceType,
                    TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(a => a.Name == "value").Id
                }
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Information);
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}