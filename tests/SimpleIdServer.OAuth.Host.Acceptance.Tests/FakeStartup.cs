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
using SimpleIdServer.OAuth.Host.Acceptance.Tests.Middlewares;
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
            services.AddMvc();
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
                opts.AddPolicy("ManageClients", p => p.RequireAssertion(b => true));
                opts.AddPolicy("ManageScopes", p => p.RequireAssertion(b => true));
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCustomAuthentication(opts =>
                {

                });
            services.AddSIDOAuth(o =>
            {
                o.SoftwareStatementTrustedParties.Add(new SoftwareStatementTrustedParty("iss", "http://localhost/custom-jwks"));
            })
                .AddClients(DefaultConfiguration.Clients)
                .AddScopes(DefaultConfiguration.Scopes)
                .AddUsers(DefaultConfiguration.Users);
            ConfigureClient(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();
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
