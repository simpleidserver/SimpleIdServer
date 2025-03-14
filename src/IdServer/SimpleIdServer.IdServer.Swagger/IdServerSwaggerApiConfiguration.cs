// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleIdServer.IdServer.Swagger;

public class IdServerSwaggerApiConfiguration
{
    private readonly SwaggerGenOptions _options;

    internal IdServerSwaggerApiConfiguration(SwaggerGenOptions options)
    {
        _options = options;
    }

    public IdServerSwaggerApiConfiguration IncludeDocumentation<T>()
    {
        var assm = typeof(T).Assembly.ManifestModule.Name.Replace(".dll", string.Empty);
        var xmlFile = $"{assm}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        _options.IncludeXmlComments(xmlPath);
        return this;
    }

    internal IdServerSwaggerApiConfiguration AddOAuthSecurity()
    {
        _options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
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
                        { Constants.DefaultScopes.ApiResources.Name, "ApiResources" },
                        { Constants.DefaultScopes.Auditing.Name, "Auditing" },
                        { Constants.DefaultScopes.Acrs.Name, "AuthenticationClassReferences" },
                        { Constants.DefaultScopes.AuthenticationMethods.Name, "AuthenticationMethods" },
                        { Constants.DefaultScopes.AuthenticationSchemeProviders.Name, "AuthenticationSchemeProviders" },
                        { Constants.DefaultScopes.CertificateAuthorities.Name, "CertificateAuthorities" },
                        { Constants.DefaultScopes.Clients.Name, "Clients" },
                        { Constants.DefaultScopes.Groups.Name, "Groups" },
                        { Constants.DefaultScopes.Provisioning.Name, "IdentityProvisioning" },
                        { Constants.DefaultScopes.Realms.Name, "Realms" },
                        { Constants.DefaultScopes.RegistrationWorkflows.Name, "RegistrationWorkflows" },
                        { Constants.DefaultScopes.Scopes.Name, "Scopes" },
                        { Constants.DefaultScopes.Users.Name, "Users" },
                        { Constants.DefaultScopes.Register.Name, "Register" },
                        { Constants.DefaultScopes.Workflows.Name, "Workflows" },
                        { Constants.DefaultScopes.RecurringJobs.Name, "RecurringJobs" },
                        { Constants.DefaultScopes.Forms.Name, "Forms" }
                    }
                }
            }
        });
        _options.AddSecurityRequirement(new OpenApiSecurityRequirement 
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