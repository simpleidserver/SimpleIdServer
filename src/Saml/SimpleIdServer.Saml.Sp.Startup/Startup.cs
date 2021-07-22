// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

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
            services.Configure<Saml2Configuration>(saml2Configuration =>
            {
                // TODO : SIGN AUTHNREQUEST
                saml2Configuration.Issuer = _configuration["Saml2:Issuer"];
                saml2Configuration.SignatureAlgorithm = _configuration["Saml2:SignatureAlgorithm"];
                saml2Configuration.SigningCertificate = certificate;
                saml2Configuration.CertificateValidationMode = (X509CertificateValidationMode)Enum.Parse(typeof(X509CertificateValidationMode), _configuration["Saml2:CertificateValidationMode"]);
                saml2Configuration.RevocationMode = (X509RevocationMode)Enum.Parse(typeof(X509RevocationMode), _configuration["Saml2:RevocationMode"]);
                saml2Configuration.SignAuthnRequest = true;
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);
                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(_configuration["Saml2:IdPMetadata"]));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }
            });
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddSaml2();
            services.AddSamlSp(o =>
            {
                o.Issuer = "urn:rp";
                o.SigningCertificate = certificate;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSaml2();
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
    }
}