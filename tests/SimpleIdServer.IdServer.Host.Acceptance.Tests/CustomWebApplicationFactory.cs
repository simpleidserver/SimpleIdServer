// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SimpleIdServer.IdServer.Host.Acceptance.Tests.Middlewares;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Stores;
using SimpleIdServer.Rp.Federation;
using SimpleIdServer.Rp.Federation.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
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
                s.RemoveAll<Helpers.IHttpClientFactory>();
                var mo = new Mock<Helpers.IHttpClientFactory>();
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
                s.AddSingleton<Helpers.IHttpClientFactory>(mo.Object);
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
    }

    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private IRpFederationEntityBuilder _rpFederationEntityBuilder;
        private RpFederationOptions _opts;
        private readonly ScenarioContext _scenarioContext;

        public FakeHttpMessageHandler(
            ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _opts = new RpFederationOptions
            {
                Client = new Domains.Client
                {
                    ApplicationType = "web",
                    RedirectionUrls = new List<string>
                    {
                        "https://openid.sunet.se/rp/callback"
                    }
                },
                ClientRegistrationTypes = ClientRegistrationType.AUTOMATIC,
                IsFederationEnabled = false,
                OrganizationName = null,
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "rpKeyId" }, SecurityAlgorithms.RsaSha256)
            };
            _rpFederationEntityBuilder = new RpFederationEntityBuilder(Microsoft.Extensions.Options.Options.Create(_opts), new FakeFederationEntityStore());
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri.AbsoluteUri;
            if(uri == "http://localhost/notificationedp")
            {
                var j = await request.Content.ReadAsStringAsync();
                _scenarioContext.Set(j, "notificationResponse");
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            }

            if(uri == "http://rp.com/.well-known/openid-federation")
            {
                var result = await _rpFederationEntityBuilder.BuildSelfIssued(new OpenidFederation.Builders.BuildFederationEntityRequest
                {
                    Credential = _opts.SigningCredentials,
                    Issuer = "rp",
                    Realm = null
                }, CancellationToken.None);
                var handler = new JsonWebTokenHandler();
                var jws = handler.CreateToken(JsonSerializer.Serialize(result), new SigningCredentials(_opts.SigningCredentials.Key, _opts.SigningCredentials.Algorithm));
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jws, System.Text.Encoding.UTF8, OpenidFederationConstants.EntityStatementContentType)
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

    public class FakeFederationEntityStore : IFederationEntityStore
    {
        public Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<FederationEntity>
            {
                new FederationEntity
                {
                    Sub = "http://idp.com"
                }
            });
        }

        public Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
