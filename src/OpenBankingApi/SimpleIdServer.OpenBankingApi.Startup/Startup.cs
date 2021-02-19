// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MediatR;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OpenBankingApi.Infrastructure.Filters;
using SimpleIdServer.OpenBankingApi.Infrastructure.Services;
using SimpleIdServer.OpenBankingApi.Persistences;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenBankingApi.Startup
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option =>
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddNewtonsoftJson();

            services.AddAuthentication()
                .AddCertificate(o =>
                {
                    o.RevocationFlag = X509RevocationFlag.EntireChain;
                    o.RevocationMode = X509RevocationMode.NoCheck;
                });
            services.AddAuthorization(opts =>
            {
                opts.AddOpenBankingAuthorization(CertificateAuthenticationDefaults.AuthenticationScheme);
            });

            services.AddMediatR(typeof(IAccountQueryRepository));
            services.AddSwaggerGen();
            services.AddOpenBankingApi();
            services.AddHostedService<EventStoreHostedService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseAuthorization();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBanking V1");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "DefaultRoute",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}