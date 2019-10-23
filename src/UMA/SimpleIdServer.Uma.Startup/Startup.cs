// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SimpleIdServer.Uma.Startup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSIDUma()
                .AddAuthentication(c =>
                {
                    c.AddCookie(UMAConstants.SignInScheme);
                    c.AddOpenIdConnect(UMAConstants.ChallengeAuthenticationScheme, options =>
                     {
                         options.ClientId = "umaClient";
                         options.ClientSecret = "umaClientSecret";
                         options.Authority = "https://localhost:60000";
                         options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                     });
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Information);
            app.UseSID();
        }
    }
}