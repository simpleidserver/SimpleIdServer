// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SimpleIdServer.Did.Crypto;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.SelfIdServer.Host.Acceptance.Tests;

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
            s.RemoveAll<SimpleIdServer.IdServer.Helpers.IHttpClientFactory>();
            var mo = new Mock<SimpleIdServer.IdServer.Helpers.IHttpClientFactory>();
            mo.Setup(m => m.GetHttpClient())
                .Returns(() =>
                {
                    var fakeHttpMessageHandler = new FakeHttpMessageHandler(_scenarioContext);
                    return new HttpClient(fakeHttpMessageHandler);
                });
            mo.Setup(m => m.GetHttpClient(It.IsAny<HttpClientHandler>()))
                .Returns(() =>
                {
                    var fakeHttpMessageHandler = new FakeHttpMessageHandler(_scenarioContext);
                    return new HttpClient(fakeHttpMessageHandler);
                });
            s.AddSingleton<SimpleIdServer.IdServer.Helpers.IHttpClientFactory>(mo.Object);
        });
    }
}

public class FakeHttpMessageHandler : DelegatingHandler
{
    private readonly ScenarioContext _scenarioContext;

    public FakeHttpMessageHandler(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri.AbsoluteUri;
        if (uri == "http://localhost/notificationedp")
        {
            var j = await request.Content.ReadAsStringAsync();
            _scenarioContext.Set(j, "notificationResponse");
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }
        if(uri == "http://clientmetadata.com/")
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var redirectUrls = new List<string>
        {
            "http://a.domain.com",
            "http://b.domain.com"
        };
        var json = JsonSerializer.Serialize(redirectUrls);
        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}
