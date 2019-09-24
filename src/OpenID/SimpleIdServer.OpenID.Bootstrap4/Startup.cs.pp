// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Options;

namespace $rootnamespace$
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
            services.Configure((OpenIDHostOptions a) =>
            {
                a.DefaultAuthenticationContextClassReferences = DefaultConfiguration.AcrLst;  
            });
            services.AddModule();
            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseModule();
        }
    }
}