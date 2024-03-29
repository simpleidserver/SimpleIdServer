﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests.Middlewares;
using System.IO;
using TechTalk.SpecFlow;

namespace SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests;

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
            s.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "Test";
            })
            .AddScheme<CustomAuthenticationHandlerOptions, CustomAuthenticationHandler>("Test", options =>
            {
                options.ScenarioContext = _scenarioContext;
            });
        });
    }
}