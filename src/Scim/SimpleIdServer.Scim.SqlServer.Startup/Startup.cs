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
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF;
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
                        ValidIssuer = "https://localhost:60000",
                        ValidAudiences = new List<string>
                        {
                            "scimClient", "gatewayClient", "provisioningClient"
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            services.AddSIDScim(_ =>
            {
                _.IgnoreUnsupportedCanonicalValues = false;
            }, massTransitOptions: x =>
            {
                x.UsingRabbitMq();
            });
            services.AddScimStoreEF(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("db"), o => o.MigrationsAssembly(migrationsAssembly));
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
                    var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMConstants.SCIMEndpoints.User, true);
                    var eidUserSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EIDUserSchema.json"), SCIMConstants.SCIMEndpoints.User);
                    var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMConstants.SCIMEndpoints.Group, true);
                    userSchema.SchemaExtensions.Add(new SCIMSchemaExtension
                    {
                        Id = Guid.NewGuid().ToString(),
                        Schema = "urn:ietf:params:scim:schemas:extension:eid:2.0:User"
                    });
                    if (!context.SCIMSchemaLst.Any())
                    {
                        context.SCIMSchemaLst.Add(userSchema);
                        context.SCIMSchemaLst.Add(groupSchema);
                        context.SCIMSchemaLst.Add(eidUserSchema);
                    }

                    if (!context.SCIMAttributeMappingLst.Any())
                    {
                        var firstAttributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                            SourceResourceType = SCIMConstants.StandardSchemas.UserSchema.ResourceType,
                            SourceAttributeSelector = "groups",
                            TargetResourceType = SCIMConstants.StandardSchemas.GroupSchema.ResourceType,
                            TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id
                        };
                        var secondAttributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
                            SourceResourceType = SCIMConstants.StandardSchemas.GroupSchema.ResourceType,
                            SourceAttributeSelector = "members",
                            TargetResourceType = SCIMConstants.StandardSchemas.UserSchema.ResourceType,
                            TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id
                        };
                        context.SCIMAttributeMappingLst.Add(firstAttributeMapping);
                        context.SCIMAttributeMappingLst.Add(secondAttributeMapping);
                    }

                    if (!context.ProvisioningConfigurations.Any())
                    {
                        context.ProvisioningConfigurations.Add(new ProvisioningConfiguration
                        {
                            Id = Guid.NewGuid().ToString(),
                            Type = ProvisioningConfigurationTypes.API,
                            ResourceType = "ScimUser",
                            UpdateDateTime = DateTime.UtcNow,
                            Records = new List<ProvisioningConfigurationRecord>
                            {
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "tokenEdp",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "https://localhost:60000/token"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "targetUrl",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "https://localhost:60000/management/users/scim"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "clientId",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "provisioningClient"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "clientSecret",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "provisioningClientSecret"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "scopes",
                                    IsArray = true,
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "manage_users"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "httpRequestTemplate",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "{ 'generate_otp': true, 'scim_id' : '{{id}}', 'content' : { 'sub' : '{{externalId}}', 'claims': { 'scim_id': '{{id}}', 'name': '{{userName}}', 'given_name' : '{{name.givenName}}', 'family_name': '{{name.familyName}}', 'middle_name': '{{claims.middleName}}' }  } }"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "bpmnHost",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "http://localhost:60007"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name ="bpmnFileId",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "c1aa1cd88cb94150c61f04b70795cb03646d43a8a65b4de005c2e6294b3aa1ff"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "messageToken",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "{ 'name': 'user',    'messageContent': {        'userId': '{{externalId}}',        'email': '{{emails[0].value}}'    }}"
                                    }
                                }
                            }
                        });
                        context.ProvisioningConfigurations.Add(new ProvisioningConfiguration
                        {
                            Id = Guid.NewGuid().ToString(),
                            Type = ProvisioningConfigurationTypes.API,
                            ResourceType = "OpenIdUser",
                            UpdateDateTime = DateTime.UtcNow,
                            Records = new List<ProvisioningConfigurationRecord>
                            {
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "tokenEdp",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "https://localhost:60000/token"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "targetUrl",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "http://localhost:60002/Users"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "clientId",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "provisioningClient"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "clientSecret",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "provisioningClientSecret"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "scopes",
                                    IsArray = true,
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "add_scim_resource"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "httpRequestTemplate",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        "{ 'schemas' : ['urn:ietf:params:scim:schemas:core:2.0:User'], 'externalId': '{{sub}}', 'userName': '{{name??sub}}', 'name': { 'givenName': '{{given_name??sub}}', 'middleName' : '{{middle_name??sub}}', 'familyName': '{{family_name??sub}}' }, 'emails': [ { 'value' : '{{email}}' } ] }"
                                    }
                                }
                            }
                        });
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}