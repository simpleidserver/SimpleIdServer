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
using SimpleIdServer.OpenID.Host.Acceptance.Tests.Middlewares;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSIDOpenID()
                .AddScopes(DefaultConfiguration.Scopes)
                .AddUsers(DefaultConfiguration.Users)
                .AddJsonWebKeys(new List<Jwt.JsonWebKey>());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCustomAuthentication(opts =>
                {

                });
            services.AddAuthorization(policy =>
            {
                policy.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
            });
            ConfigureClient(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            mo.Setup(m => m.GetHttpClient())
                .Returns(() =>
                {
                    var url = "http://domain.com/sector";
                    var jArr = new JArray
                    {
                        "http://a.domain.com", "http://b.domain.com"
                    };
                    var dic = new Dictionary<string, HttpResponseMessage>();
                    dic.Add(url, new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(jArr.ToString(), Encoding.UTF8, "application/json") });
                    var fakeHttpMessageHandler = new FakeHttpMessageHandler(dic);
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