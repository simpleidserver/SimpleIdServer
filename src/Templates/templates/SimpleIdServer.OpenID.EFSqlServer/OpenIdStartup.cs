// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OpenID.EF;
using SimpleIdServer.OpenID;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace SimpleIdServer.OpenID.EFSqlServer
{
    public class OpenIdStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var sigJsonWebKey = ExtractJsonWebKeyFromRSA("openid_key.txt", "RS256");
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddAuthorization(opts => opts.AddDefaultOAUTHAuthorizationPolicy());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddCommonSID();
            services
                .AddSIDOpenID()
                .AddOpenIDEF(opt => opt.UseSqlServer("<<CONNECTIONSTRING>>", o => o.MigrationsAssembly(typeof(OpenIdStartup).GetTypeInfo().Assembly.GetName().Name)))
                .AddLoginPasswordAuthentication();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            app.UseForwardedHeaders();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSIDOpenId();
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

        private static JsonWebKey ExtractJsonWebKeyFromRSA(string fileName, string algName)
        {
            using (var rsa = RSA.Create())
            {
                var json = File.ReadAllText(fileName);
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                rsa.Import(dic);
                return new JsonWebKeyBuilder().NewSign("1", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, algName).Build();
            }
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<OpenIdDBContext>())
                {
                    context.Database.Migrate();
                    if (!context.OAuthScopes.Any())
                    {
                        context.OAuthScopes.AddRange(OpenIdDefaultConfiguration.Scopes);
                    }

                    if (!context.Users.Any())
                    {
                        context.Users.AddRange(OpenIdDefaultConfiguration.Users);
                    }

                    if (!context.Acrs.Any())
                    {
                        context.Acrs.AddRange(OpenIdDefaultConfiguration.AcrLst);
                    }

                    if (!context.OpenIdClients.Any())
                    {
                        context.OpenIdClients.AddRange(OpenIdDefaultConfiguration.GetClients());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}