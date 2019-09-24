// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Host.Acceptance.Tests.Middlewares;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using IHttpClientFactory = SimpleIdServer.OAuth.Infrastructures.IHttpClientFactory;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddModule();
            services.AddLogging();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCustomAuthentication(opts =>
                {
                    
                });
            services.Configure<OAuthHostOptions>(a =>
            {
                a.SoftwareStatementTrustedParties.Add(new SoftwareStatementTrustedParty("iss", "http://localhost/custom-jwks"));
            });
            services.AddAuthorization(policy =>
            {
                policy.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
            });
            ConfigureClient(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ConfigureRepositories(app);
            app.UseAuthentication();
            app.UseModule();
        }

        private static void ConfigureClient(IServiceCollection services)
        {
            var mo = new Mock<IHttpClientFactory>();
            mo.Setup(m => m.GetHttpClient())
                .Returns(() =>
                {
                    var url = "http://localhost/custom-jwks";
                    var jwks = FakeJwks.GetInstance().Jwks;
                    var keys = new JArray();
                    foreach (var jsonWebKey in jwks)
                    {
                        var key = new JObject();
                        keys.Add(jsonWebKey.GetPublicJwt());
                    }

                    var result = new JObject
                    {
                        { "keys", keys }
                    };
                    var dic = new Dictionary<string, HttpResponseMessage>();
                    dic.Add(url, new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(result.ToString(), Encoding.UTF8, "application/json") });
                    var fakeHttpMessageHandler = new FakeHttpMessageHandler(dic);
                    return new HttpClient(fakeHttpMessageHandler);
                });
            services.RemoveAll<IHttpClientFactory>();
            services.AddSingleton<IHttpClientFactory>(mo.Object);
        }

        private static IApplicationBuilder ConfigureRepositories(IApplicationBuilder app)
        {
            var oauthUserRepository = app.ApplicationServices.GetRequiredService<IOAuthUserCommandRepository>();
            var oauthClientRepository = app.ApplicationServices.GetRequiredService<IOAuthClientCommandRepository>();
            var oauthScopeRepository = app.ApplicationServices.GetRequiredService<IOAuthScopeCommandRepository>();
            foreach (var user in DefaultConfiguration.Users)
            {
                oauthUserRepository.Add(user);
            }

            foreach (var client in DefaultConfiguration.Clients)
            {
                oauthClientRepository.Add(client);
            }

            foreach (var scope in DefaultConfiguration.Scopes)
            {
                oauthScopeRepository.Add(scope);
            }

            return app;
        }

        private static void HandleCustomJwks(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var jwks = FakeJwks.GetInstance().Jwks;
                var keys = new JArray();
                foreach (var jsonWebKey in jwks)
                {
                    var key = new JObject();
                    keys.Add(jsonWebKey.GetPublicJwt());
                }

                var result = new JObject
                {
                    { "keys", keys }
                };
                var data = Encoding.UTF8.GetBytes(result.ToString());
                context.Response.ContentType = "application/json";
                await context.Response.Body.WriteAsync(data, 0, data.Length);
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder authBuilder, Action<CustomAuthenticationHandlerOptions> callback)
        {
            authBuilder.AddScheme<CustomAuthenticationHandlerOptions, CustomAuthenticationHandler>(CookieAuthenticationDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme, callback);
            return authBuilder;
        }
    }
}
