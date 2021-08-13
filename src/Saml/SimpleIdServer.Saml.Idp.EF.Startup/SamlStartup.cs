// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Idp.EF.Startup
{
    public class SamlStartup
    {
        private readonly IConfiguration _configuration;

        public SamlStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(SamlStartup).GetTypeInfo().Assembly.GetName().Name;
            var certificate = new X509Certificate2(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "localhost.pfx"), "password");
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddCommonSID();
            services.AddSamlIdpEF(opt =>
            {
                opt.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=SamlIdp;Integrated Security=True", o => o.MigrationsAssembly(migrationsAssembly));
            });
            services.AddSamlIdp(opt =>
            {
                opt.SigningCertificate = certificate;
                opt.SignatureAlg = SignatureAlgorithms.RSASHA256;
                opt.CanonicalizationMethod = CanonicalizationMethods.C14;
            });
            services.AddSamlLoginPawdAuth();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_configuration.GetChildren().Any(i => i.Key == "pathBase"))
            {
                app.UsePathBase(_configuration["pathBase"]);
            }

            InitializeDatabase(app);
            app.UseForwardedHeaders();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseStaticFiles();
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

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SamlIdpDBContext>())
                {
                    context.Database.Migrate();
                    if (!context.Users.Any())
                    {
                        context.Users.AddRange(SamlDefaultConfiguration.Users);
                    }

                    if (!context.RelyingParties.Any())
                    {
                        context.RelyingParties.AddRange(SamlDefaultConfiguration.RelyingParties);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}