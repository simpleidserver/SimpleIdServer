// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.OAuth.Host.Acceptance.Tests.Middlewares;
using SimpleIdServer.OAuth.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IHttpClientFactory = SimpleIdServer.OAuth.Infrastructures.IHttpClientFactory;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.
                AddMvc(option => option.EnableEndpointRouting = false)
                .AddApplicationPart(typeof(RegistrationController).Assembly)
                .AddNewtonsoftJson();
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
                options.HttpsPort = 8080;
            });
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
                opts.AddPolicy("ManageClients", p => p.RequireAssertion(b => true));
                opts.AddPolicy("ManageScopes", p => p.RequireAssertion(b => true));
                opts.AddPolicy("ManageUsers", p => p.RequireAssertion(b => true));
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCustomAuthentication(opts =>
                {

                })
                .AddCertificate(o =>
                {
                    o.AllowedCertificateTypes = CertificateTypes.All;
                    o.RevocationFlag = X509RevocationFlag.EntireChain;
                    o.RevocationMode = X509RevocationMode.NoCheck;
                });
            services.AddSIDOAuth(o =>
            {
                o.MtlsEnabled = true;
                o.SoftwareStatementTrustedParties.Add(new SoftwareStatementTrustedParty("iss", "http://localhost/custom-jwks"));
            })
                .AddClients(DefaultConfiguration.Clients)
                .AddScopes(DefaultConfiguration.Scopes)
                .AddUsers(DefaultConfiguration.Users);
            ConfigureClient(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseSIDOauth();
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
