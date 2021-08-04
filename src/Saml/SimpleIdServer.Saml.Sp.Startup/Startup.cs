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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddSamlSp(o =>
            {
                o.SPId = "urn:sp";
                o.SPName = "SP Saml2";
                o.SigningCertificate = certificate;
                o.AuthnRequestSigned = true;
                o.WantAssertionSigned = true;
                o.SignatureAlg = SignatureAlgorithms.RSASHA256;
                o.CanonicalizationMethod = CanonicalizationMethods.C14;
                o.IdpMetadataUrl = "http://localhost:7000/saml/metadata";
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSamlSp();
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
}