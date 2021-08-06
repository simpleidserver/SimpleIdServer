// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Sp.Startup
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var certificate = new X509Certificate2(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "localhost.pfx"), "password");
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddCookie("ExternalAuthentication")
                .AddSamlSp(opts =>
                {
                    ConfigureLocalSamlIdp(opts, certificate);
                });
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
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

        private static void ConfigureLocalSamlIdp(SamlSpOptions opts, X509Certificate2 certificate)
        {
            opts.SignInScheme = "ExternalAuthentication";
            opts.SPId = "urn:sp";
            opts.SigningCertificate = certificate;
            opts.AuthnRequestSigned = true;
            opts.WantsResponseSigned = true;
            opts.WantAssertionSigned = true;
            opts.SignatureAlg = SignatureAlgorithms.RSASHA256;
            opts.CanonicalizationMethod = CanonicalizationMethods.C14;
            opts.IdpMetadataUrl = "http://localhost:7000/saml/metadata";
        }

        private static void ConfigureKeycloackSamlId(SamlSpOptions opts, X509Certificate2 certificate)
        {
            opts.SignInScheme = "ExternalAuthentication";
            opts.SPId = "urn:keycloacksp";
            opts.SigningCertificate = certificate;
            opts.AuthnRequestSigned = true;
            opts.WantsResponseSigned = true;
            opts.WantAssertionSigned = true;
            opts.SignatureAlg = SignatureAlgorithms.RSASHA256;
            opts.CanonicalizationMethod = CanonicalizationMethods.C14;
            opts.IdpMetadataUrl = "http://localhost:8080/auth/realms/master/protocol/saml/descriptor";
        }
    }
}