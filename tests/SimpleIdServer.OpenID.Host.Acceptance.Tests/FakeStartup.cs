// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OpenID.Api.BCAuthorize;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Host.Acceptance.Tests.Middlewares;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using TechTalk.SpecFlow;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public FakeStartup() { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o => o.EnableEndpointRouting = false)
                .AddNewtonsoftJson();
            services.AddSIDOpenID()
                .AddClients(new List<OpenIdClient>(), DefaultConfiguration.Scopes)
                .AddUsers(DefaultConfiguration.Users)
                .AddJsonWebKeys(new List<Jwt.JsonWebKey>());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCustomAuthentication(opts =>
                {

                });
            services.AddAuthorization(policy =>
            {
                policy.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
                policy.AddPolicy("ManageClients", p => p.RequireAssertion(_ => true));
                policy.AddPolicy("ManageScopes", p => p.RequireAssertion(_ => true));
            });
            ConfigureClient(services);
            services.RemoveAll<IBCNotificationService>();
            services.AddTransient<IBCNotificationService, FakeNotificationService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "AreaRoute",
                  template: "{area}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "DefaultRoute",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void ConfigureClient(IServiceCollection services)
        {
            var mo = new Mock<OAuth.Infrastructures.IHttpClientFactory>();
            var scenarioContext = services.BuildServiceProvider().GetService<ScenarioContext>();
            mo.Setup(m => m.GetHttpClient())
                .Returns(() =>
                {
                    var url = "http://domain.com/sector";
                    var jArr = new JArray
                    {
                        "http://a.domain.com", 
                        "http://b.domain.com"
                    };
                    var dic = new Dictionary<string, HttpResponseMessage>();
                    dic.Add(url, new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(jArr.ToString(), Encoding.UTF8, "application/json") });
                    dic.Add("https://localhost:8080/pushNotificationEdp", new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(jArr.ToString(), Encoding.UTF8, "application/json") });
                    var fakeHttpMessageHandler = new FakeHttpMessageHandler(scenarioContext, dic);
                    return new HttpClient(fakeHttpMessageHandler);
                });
            services.RemoveAll<OAuth.Infrastructures.IHttpClientFactory>();
            services.AddSingleton<OAuth.Infrastructures.IHttpClientFactory>(mo.Object);
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