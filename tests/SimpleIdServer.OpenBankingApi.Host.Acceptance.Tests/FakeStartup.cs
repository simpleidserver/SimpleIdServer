// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.OpenBankingApi.AccountAccessContents;
using SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests.Middlewares;
using SimpleIdServer.OpenBankingApi.Infrastructure.Authorizations;
using SimpleIdServer.OpenBankingApi.Infrastructure.Services;
using SimpleIdServer.OpenBankingApi.Persistences;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.
                AddMvc(option => option.EnableEndpointRouting = false)
                .AddApplicationPart(typeof(RegistrationController).Assembly)
                .AddApplicationPart(typeof(AccountAccessContentsController).Assembly)
                .AddNewtonsoftJson();
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
                options.HttpsPort = 8080;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCustomAuthentication(opts =>
                {

                })
                .AddCertificate(o =>
                {
                    o.RevocationFlag = X509RevocationFlag.EntireChain;
                    o.RevocationMode = X509RevocationMode.NoCheck;
                });

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
                opts.AddPolicy("ManageClients", p => p.RequireAssertion(b => true));
                opts.AddPolicy("ManageScopes", p => p.RequireAssertion(b => true));
                opts.AddOpenBankingAuthorization(CertificateAuthenticationDefaults.AuthenticationScheme);
            });

            services.AddMediatR(typeof(IAccountQueryRepository));
            services.AddOpenBankingApi();
            services.AddHostedService<EventStoreHostedService>();
            services.AddSIDOAuth(o =>
            {
                o.MtlsEnabled = true;
            })
                .AddScopes(DefaultConfiguration.Scopes);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseSIDOauth();
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
