// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleIdServer.OAuth.Startup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSIDOAuth(o =>
            {
                o.ClientSecretExpirationInSeconds = 2;
            })
            .AddClients(DefaultConfiguration.Clients)
            .AddScopes(DefaultConfiguration.Scopes);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSID();
        }
    }
}