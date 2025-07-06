// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SimpleIdServer.IdServer.Swagger.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleIdServer.IdServer.Swagger;

public class IdServerSwaggerApiConfiguration
{
    private readonly IServiceCollection _services;
    private readonly List<string> _xmlPaths;
    private bool _addDocumentFilter;

    internal IdServerSwaggerApiConfiguration(IServiceCollection services)
    {
        _services = services;
        _xmlPaths = new List<string>();
    }

    internal SwaggerGenOptions Options
    {
        get; set;
    }

    public IdServerSwaggerApiConfiguration IncludeDocumentation<T>()
    {
        var assm = typeof(T).Assembly.ManifestModule.Name.Replace(".dll", string.Empty);
        var xmlFile = $"{assm}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        _xmlPaths.Add(xmlPath);
        return this;
    }

    public IdServerSwaggerApiConfiguration ExcludeDocumentations(params string[] filters)
    {
        _services.AddSingleton(new ExcludeEndpointsConfig
        {
            SegmentsToExclude = filters.ToArray()
        });
        _addDocumentFilter = true;
        return this;
    }

    internal void Configure()
    {
        foreach(var xmlPath in _xmlPaths)
        {
            Options.IncludeXmlComments(xmlPath);
        }

        if(_addDocumentFilter)
        {
            Options.DocumentFilter<ExcludeEndpointsDocumentFilter>();
        }
    }

    internal IdServerSwaggerApiConfiguration AddOAuthSecurity()
    {
        Options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = null,
                    TokenUrl = null,
                    Scopes = new Dictionary<string, string>
                    {
                        { Config.DefaultScopes.ApiResources.Name, "ApiResources" },
                        { Config.DefaultScopes.Auditing.Name, "Auditing" },
                        { Config.DefaultScopes.Acrs.Name, "AuthenticationClassReferences" },
                        { Config.DefaultScopes.AuthenticationMethods.Name, "AuthenticationMethods" },
                        { Config.DefaultScopes.AuthenticationSchemeProviders.Name, "AuthenticationSchemeProviders" },
                        { Config.DefaultScopes.CertificateAuthorities.Name, "CertificateAuthorities" },
                        { Config.DefaultScopes.Clients.Name, "Clients" },
                        { Config.DefaultScopes.Groups.Name, "Groups" },
                        { Config.DefaultScopes.Provisioning.Name, "IdentityProvisioning" },
                        { Config.DefaultScopes.Realms.Name, "Realms" },
                        { Config.DefaultScopes.RegistrationWorkflows.Name, "RegistrationWorkflows" },
                        { Config.DefaultScopes.Scopes.Name, "Scopes" },
                        { Config.DefaultScopes.Users.Name, "Users" },
                        { Config.DefaultScopes.Register.Name, "Register" },
                        { Config.DefaultScopes.Workflows.Name, "Workflows" },
                        { Config.DefaultScopes.RecurringJobs.Name, "RecurringJobs" },
                        { Config.DefaultScopes.Forms.Name, "Forms" }
                    }
                }
            }
        });
        Options.AddSecurityRequirement(new OpenApiSecurityRequirement 
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "OAuth2",
                        Type = ReferenceType.SecurityScheme
                    }
                }, new List<string>()
            }
        });

        return this;
    }
}