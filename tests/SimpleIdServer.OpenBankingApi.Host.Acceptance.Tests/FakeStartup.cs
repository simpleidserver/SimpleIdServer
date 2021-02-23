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
using Newtonsoft.Json.Serialization;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.OpenBankingApi.AccountAccessContents;
using SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests.Middlewares;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Api.UserInfo;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

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
                .AddApplicationPart(typeof(UserInfoController).Assembly)
                .AddApplicationPart(typeof(AccountAccessContentsController).Assembly)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
                options.HttpsPort = 8080;
            });

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
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

            services.AddMediatR(typeof(IAccountRepository));
            services.AddSIDOpenID(opt =>
            {
                opt.IsLocalhostAllowed = true;
                opt.IsRedirectionUrlHTTPSRequired = false;
                opt.IsInitiateLoginUriHTTPSRequired = true;
            }, opt =>
            {
                opt.MtlsEnabled = true;
                opt.DefaultScopes = new List<string>
                    {
                        SIDOpenIdConstants.StandardScopes.Profile.Name,
                        SIDOpenIdConstants.StandardScopes.Email.Name,
                        SIDOpenIdConstants.StandardScopes.Address.Name,
                        SIDOpenIdConstants.StandardScopes.Phone.Name
                    };
            })
                .AddUsers(DefaultConfiguration.Users)
                .AddClients(new List<OpenID.Domains.OpenIdClient>(), DefaultConfiguration.Scopes);
            services.AddOpenBankingApi();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseSIDOauth();
            app.UseOpenBankingAPI();
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
