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

    public IdServerSwaggerApiConfiguration AddOAuthSecurity()
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
                        { Constants.StandardScopes.ApiResources.Name, "ApiResources" },
                        { Constants.StandardScopes.Auditing.Name, "Auditing" },
                        { Constants.StandardScopes.Acrs.Name, "AuthenticationClassReferences" },
                        { Constants.StandardScopes.AuthenticationMethods.Name, "AuthenticationMethods" },
                        { Constants.StandardScopes.AuthenticationSchemeProviders.Name, "AuthenticationSchemeProviders" },
                        { Constants.StandardScopes.CertificateAuthorities.Name, "CertificateAuthorities" },
                        { Constants.StandardScopes.Clients.Name, "Clients" },
                        { Constants.StandardScopes.Groups.Name, "Groups" },
                        { Constants.StandardScopes.Provisioning.Name, "IdentityProvisioning" },
                        { Constants.StandardScopes.Realms.Name, "Realms" },
                        { Constants.StandardScopes.RegistrationWorkflows.Name, "RegistrationWorkflows" },
                        { Constants.StandardScopes.Scopes.Name, "Scopes" },
                        { Constants.StandardScopes.Users.Name, "Users" }
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