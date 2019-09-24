// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Options;

namespace SimpleIdServer.OAuth.Startup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OAuthHostOptions>(a =>
            {
                a.DefaultOAuthClients = DefaultConfiguration.Clients;
                a.DefaultUsers = DefaultConfiguration.Users;
            });
            services.AddModule();
            services.AddLogging();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
                {
                    opts.LoginPath = "/pwd/Authenticate";
                });
            services.AddAuthorization(policy =>
            {
                policy.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();
            app.UseModule();
        }
    }
}