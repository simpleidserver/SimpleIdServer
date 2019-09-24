using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.ClaimsEnrichers;
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
            services.Configure<OAuthHostOptions>(a =>
            {
                a.DefaultOAuthScopes = DefaultConfiguration.Scopes;
                a.DefaultUsers = DefaultConfiguration.Users;
            });
            services.AddModule();
            // ConfigureDistributedClaims(services);
            // ConfigureAggregateClaimsSource(services);
            services.AddLogging();
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
            app.UseModule();
        }

        private static void ConfigureDistributedClaims(IServiceCollection services)
        {
            services.AddAggregateHttpClaimsSource(new AggregateHttpClaimsSourceOptions("posts", "https://samples.openweathermap.org/data/2.5/weather?q=London,uk&appid=b6907d289e10d714a6e88b30761fae22"));
        }

        private static void ConfigureAggregateClaimsSource(IServiceCollection services)
        {
            services.AddDistributeHttpClaimsSource(new ClaimsEnrichers.DistributeHttpClaimsSourceOptions("name_1", "url_1", "apitoken_1", new[] { "str1" }));
            services.AddDistributeHttpClaimsSource(new ClaimsEnrichers.DistributeHttpClaimsSourceOptions("name_2", "url_2", "apitoken_2", new[] { "str2" }));
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