﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Domains.Builders;
using SimpleIdServer.Scim.Host.Acceptance.Tests;
using SimpleIdServer.Scim.Infrastructure.ValueProviders;
using System;
using System.Collections.Generic;
using System.Linq;

void ConfigureServices(IServiceCollection services)
{
    var userSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", SCIMResourceTypes.User, "User Account", true)
       .AddStringAttribute("userName", required: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE, caseExact: true, uniqueness: SCIMSchemaAttributeUniqueness.SERVER)
       .AddComplexAttribute("name", c =>
       {
           c.AddStringAttribute("formatted", description: "The full name");
           c.AddStringAttribute("familyName", description: "The family name");
           c.AddStringAttribute("givenName", description: "The given name");
       }, description: "The components of the user's real name.")
       .AddStringAttribute("roles", multiValued: true)
       .AddDecimalAttribute("age")
       .AddDateTimeAttribute("birthDate")
       .AddBooleanAttribute("active")
       .AddStringAttribute("duplicateAttr")
       .AddIntAttribute("nbPoints")
       .AddStringAttribute("organizationId")
       .AddBinaryAttribute("eidCertificate")
       .AddComplexAttribute("subImmutableComplex", opt =>
       {
           opt.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.IMMUTABLE);
           opt.AddStringAttribute("type", mutability: SCIMSchemaAttributeMutabilities.IMMUTABLE);
       }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
       .AddComplexAttribute("complexImmutable", opt =>
       {
           opt.AddStringAttribute("value");
           opt.AddStringAttribute("type");
       }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.IMMUTABLE)
       .AddComplexAttribute("emails", opt =>
       {
           opt.AddStringAttribute("value");
           opt.AddStringAttribute("display");
           opt.AddBooleanAttribute("primary");
           opt.AddStringAttribute("type");
       }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
       .AddStringAttribute("immutable", mutability: SCIMSchemaAttributeMutabilities.IMMUTABLE)
       .AddComplexAttribute("groups", opt =>
       {
           opt.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.READONLY);
           opt.AddStringAttribute("display", mutability: SCIMSchemaAttributeMutabilities.READONLY);
           opt.AddStringAttribute("type", new List<string> { "direct", "indirect" }, mutability: SCIMSchemaAttributeMutabilities.READONLY);
       }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READONLY)
       .AddComplexAttribute("phones", opt =>
       {
           opt.AddStringAttribute("phoneNumber", description: "Phone number");
           opt.AddStringAttribute("type", description: "Type");
       }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
       .AddComplexAttribute("adRoles", opt =>
       {
           opt.AddStringAttribute("value", description: "Value");
           opt.AddStringAttribute("display", description: "Display");
       }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
       .AddComplexAttribute("scores", opt =>
       {
           opt.AddComplexAttribute("math", sopt =>
           {
               sopt.AddIntAttribute("score");
           }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE);
       }, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
       .AddStringAttribute("type", canonicalValues: new List<string>
       {
               "manager",
               "employee"
       })
       .AddStringAttribute("org", defaultValue: new List<string> { "ENTREPRISE" }, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
       .AddSCIMSchemaExtension("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User", true)
       .Build();
    var enterpriseUser = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User", "EnterpriseUser", "EnterpriseUser", string.Empty, false)
        .AddStringAttribute("employeeNumber", required: true)
        .AddStringAttribute("duplicateAttr")
        .Build();
    var entitlementSchema = SCIMSchemaBuilder.Create("urn:entitlement", "Entitlement", "Entitlement", string.Empty, true)
        .AddStringAttribute("displayName")
        .AddComplexAttribute("members", opt =>
        {
            opt.AddStringAttribute("value");
            opt.AddReferenceAttribute("$ref");
            opt.AddStringAttribute("type");
            opt.AddStringAttribute("display");
        }, multiValued: true)
        .Build();
    var customUserSchema = SCIMSchemaBuilder.Create("urn:customuser", "CustomUser", "CustomUser", string.Empty, true)
        .AddStringAttribute("userName", required: true)
        .AddComplexAttribute("emails", o =>
        {
            o.AddStringAttribute("value", required: true);
        }, multiValued: true)
        .AddComplexAttribute("entitlements", opt =>
        {
            opt.AddStringAttribute("value");
            opt.AddReferenceAttribute("$ref");
            opt.AddStringAttribute("type");
            opt.AddStringAttribute("display");
        }, multiValued: true)
        .Build();
    var schemas = new List<SCIMSchema>
        {
            userSchema,
            enterpriseUser,
            StandardSchemas.GroupSchema,
            entitlementSchema,
            customUserSchema
        };
    var attributesMapping = new List<SCIMAttributeMapping>
        {
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                SourceResourceType = userSchema.ResourceType,
                SourceAttributeSelector = "groups",
                TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
                TargetAttributeId = StandardSchemas.GroupSchema.Attributes.First(a => a.Name == "members").Id
            },
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = StandardSchemas.GroupSchema.Attributes.First(a => a.Name == "members").Id,
                SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                SourceAttributeSelector = "members",
                TargetResourceType = userSchema.ResourceType,
                TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                Mode = Mode.PROPAGATE_INHERITANCE
            },
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = StandardSchemas.GroupSchema.Attributes.First(a => a.Name == "members").Id,
                SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
                SourceAttributeSelector = "members",
                TargetResourceType = StandardSchemas.GroupSchema.ResourceType
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
        };

    services.AddMvc(o =>
    {
        o.EnableEndpointRouting = false;
        o.ValueProviderFactories.Insert(0, new SeparatedQueryStringValueProviderFactory(","));
    }).AddNewtonsoftJson(o => { });
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
    // services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
    services.AddAuthentication(SCIMConstants.AuthenticationScheme).AddCustomAuthentication(c => { });
    services.AddSIDScim(o =>
    {
        o.MaxOperations = 3;
        o.IgnoreUnsupportedCanonicalValues = false;
        o.MergeExtensionAttributes = true;
    })
        .AddSchemas(schemas)
        .AddAttributeMapping(attributesMapping);
}

void Configure(WebApplication app)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMvc(o =>
    {
        o.UseStandardScimEdp("CustomUsers", false);
        o.UseStandardScimEdp("Entitlements", false);
        o.UseScim();
    });
}

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services);
var app = builder.Build();
Configure(app);
app.Run();

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder authBuilder, Action<AuthenticationSchemeOptions> callback)
    {
        authBuilder.AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(SCIMConstants.AuthenticationScheme, SCIMConstants.AuthenticationScheme, callback);
        return authBuilder;
    }
}
