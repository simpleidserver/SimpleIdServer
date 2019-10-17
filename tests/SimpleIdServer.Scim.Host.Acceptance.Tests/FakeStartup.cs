using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var userSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", "User Account")
               .AddStringAttribute("userName", caseExact: true, uniqueness: SCIMSchemaAttributeUniqueness.SERVER)
               .AddComplexAttribute("name", c =>
               {
                   c.AddStringAttribute("formatted", description: "The full name");
                   c.AddStringAttribute("familyName", description: "The family name");
                   c.AddStringAttribute("givenName", description: "The given name");
               }, description: "The components of the user's real name.")
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
               .AddStringAttribute("org", defaultValue: new List<string> { "ENTREPRISE" }, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
               .Build();
            var schemas = new List<SCIMSchema>
            {
                userSchema
            };
            schemas.AddRange(SCIMConstants.StandardSchemas.GroupSchemas);
            services.AddSIDScim(o =>
            {
                o.MaxOperations = 3;
            }).AddAuthentication(a =>
            {
                a.AddCustomAuthentication(c => { });
            }).AddSchemas(schemas);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSID();
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
