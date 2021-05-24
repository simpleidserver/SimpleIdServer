// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Infrastructure.ValueProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var userSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", SCIMConstants.SCIMEndpoints.User, "User Account", true)
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
               .AddBinaryAttribute("eidCertificate")
               .AddStringAttribute("immutable", mutability: SCIMSchemaAttributeMutabilities.IMMUTABLE)
               .AddComplexAttribute("groups", opt =>
               {
                   opt.AddStringAttribute("value", mutability: SCIMSchemaAttributeMutabilities.READONLY);
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
            var enterpriseUser = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User", "EnterpriseUser", "Enterprise user", string.Empty, false)
                .AddStringAttribute("employeeNumber", required: true)
                .AddStringAttribute("duplicateAttr")
                .Build();
            var schemas = new List<SCIMSchema>
            {
                userSchema,
                enterpriseUser,
                SCIMConstants.StandardSchemas.GroupSchema
            };
            var attributesMapping = new List<SCIMAttributeMapping>
            {
                new SCIMAttributeMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
                    SourceResourceType = userSchema.ResourceType,
                    SourceAttributeSelector = "groups",
                    TargetResourceType = SCIMConstants.StandardSchemas.GroupSchema.ResourceType,
                    TargetAttributeId = SCIMConstants.StandardSchemas.GroupSchema.Attributes.First(a => a.Name == "members").SubAttributes.First(a => a.Name == "value").Id
                }
            };
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
                o.ValueProviderFactories.Insert(0, new SeparatedQueryStringValueProviderFactory(","));
            }).AddNewtonsoftJson(o => { });
            services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
            services.AddAuthentication(SCIMConstants.AuthenticationScheme).AddCustomAuthentication(c => { });
            services.AddSIDScim(o =>
            {
                o.MaxOperations = 3;
                o.IgnoreUnsupportedCanonicalValues = false;
            })
                .AddSchemas(schemas)
                .AddAttributeMapping(attributesMapping);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMvc();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder authBuilder, Action<AuthenticationSchemeOptions> callback)
        {
            authBuilder.AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>(SCIMConstants.AuthenticationScheme, SCIMConstants.AuthenticationScheme, callback);
            return authBuilder;
        }
    }
}
