// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Saml.Idp.SSO.Apis;

namespace SimpleIdServer.Saml.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(policy =>
            {
                policy.AddPolicy("ManageRelyingParties", p => p.RequireAssertion(_ => true));
            });
            services.
                AddMvc(option => option.EnableEndpointRouting = false)
                .AddApplicationPart(typeof(SingleSignOnController).Assembly)
                .AddNewtonsoftJson();
            services.AddSamlIdp();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSamlIdp();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
