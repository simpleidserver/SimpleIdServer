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
using SimpleIdServer.Scim.Domains;
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
            // services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
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
                        ValidIssuer = Configuration["OpenIdUrl"],
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
            });
            services.AddScimStoreEF(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("db"), o =>
                {
                    o.MigrationsAssembly(migrationsAssembly);
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
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
                    var userSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
                    var eidUserSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EIDUserSchema.json"), SCIMResourceTypes.User);
                    var groupSchema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "GroupSchema.json"), SCIMResourceTypes.Group, true);
                    var entrepriseUser = SCIMSchemaExtractor.Extract(Path.Combine(basePath, "EnterpriseUser.json"), SCIMResourceTypes.User);
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
                            TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id
                        };
                        var thirdAttributeMapping = new SCIMAttributeMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            SourceAttributeId = entrepriseUser.Attributes.First(a => a.Name == "manager").Id,
                            SourceResourceType = StandardSchemas.UserSchema.ResourceType,
                            SourceAttributeSelector = "manager",
                            TargetResourceType = StandardSchemas.UserSchema.ResourceType
                        };
                        context.SCIMAttributeMappingLst.Add(firstAttributeMapping);
                        context.SCIMAttributeMappingLst.Add(secondAttributeMapping);
                        context.SCIMAttributeMappingLst.Add(thirdAttributeMapping);
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
                                        $"{Configuration["OpenIdUrl"]}/token"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "targetUrl",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        $"{Configuration["OpenIdUrl"]}/management/users/scim"
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
                                        Configuration["BpmnUrl"]
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
                                        $"{Configuration["OpenIdUrl"]}/token"
                                    }
                                },
                                new ProvisioningConfigurationRecord
                                {
                                    Name = "targetUrl",
                                    Type = ProvisioningConfigurationRecordTypes.STRING,
                                    ValuesString = new List<string>
                                    {
                                        $"{Configuration["ScimUrl"]}/Users"
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