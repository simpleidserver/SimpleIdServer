// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SimpleIdServer.IdServer.Host.Acceptance.Tests.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests
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
                s.RemoveAll<Infrastructures.IHttpClientFactory>();
                var mo = new Mock<Infrastructures.IHttpClientFactory>();
                mo.Setup(m => m.GetHttpClient())
                    .Returns(() =>
                    {
                        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
                        return new HttpClient(fakeHttpMessageHandler);
                    });
                mo.Setup(m => m.GetHttpClient(It.IsAny<HttpClientHandler>()))
                    .Returns(() =>
                    {
                        var fakeHttpMessageHandler = new FakeHttpMessageHandler();
                        return new HttpClient(fakeHttpMessageHandler);
                    });
                s.AddSingleton<Infrastructures.IHttpClientFactory>(mo.Object);
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
                        var certificate = _scenarioContext.Get<X509Certificate2>(key);
                        ctx.Connection.ClientCertificate = certificate;
                    }
                    return nxt();
                });
                next(builder);
            };
        }

        private static X509ChainPolicy BuildChainPolicy(X509Certificate2 certificate, bool isCertificateSelfSigned)
        {
            // Now build the chain validation options.
            X509RevocationFlag revocationFlag = X509RevocationFlag.EntireChain;
            X509RevocationMode revocationMode = X509RevocationMode.NoCheck;

            if (isCertificateSelfSigned)
            {
                // Turn off chain validation, because we have a self signed certificate.
                revocationFlag = X509RevocationFlag.EntireChain;
                revocationMode = X509RevocationMode.NoCheck;
            }

            var chainPolicy = new X509ChainPolicy
            {
                RevocationFlag = revocationFlag,
                RevocationMode = revocationMode,
            };

            if (isCertificateSelfSigned)
            {
                chainPolicy.VerificationFlags |= X509VerificationFlags.AllowUnknownCertificateAuthority;
                chainPolicy.VerificationFlags |= X509VerificationFlags.IgnoreEndRevocationUnknown;
                chainPolicy.ExtraStore.Add(certificate);
            }
            else
            {
                chainPolicy.TrustMode = X509ChainTrustMode.System;
            }

            return chainPolicy;
        }
    }

    public class FakeHttpMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var redirectUrls = new List<string>
            {
                "http://a.domain.com",
                "http://b.domain.com"
            };
            var json = JsonSerializer.Serialize(redirectUrls);
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }
}
