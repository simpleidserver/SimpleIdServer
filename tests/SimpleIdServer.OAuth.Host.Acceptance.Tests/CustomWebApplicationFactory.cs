// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OAuth.Host.Acceptance.Tests.Middlewares;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly ScenarioContext _scenarioContext;

        public CustomWebApplicationFactory(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureTestServices(s =>
            {
                s.AddAuthentication(CustomAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<CustomAuthenticationHandlerOptions, CustomAuthenticationHandler>(CustomAuthenticationHandler.AuthenticationScheme, options => {
                        options.ScenarioContext = _scenarioContext;
                    });
                s.AddSingleton<IStartupFilter>(new CertificateStartupFilter(_scenarioContext));
            });
        }
    }

    public class CertificateStartupFilter : IStartupFilter
    {
        private readonly ScenarioContext _scenarioContext;

        public CertificateStartupFilter(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.Use((ctx, nxt) =>
                {
                    if (ctx.Request.Headers.TryGetValue("X-Testing-ClientCert", out var key))
                    {
                        ctx.Connection.ClientCertificate = _scenarioContext.Get<X509Certificate2>(key);
                    }
                    return nxt();
                });
                next(builder);
            };
        }
    }
}
