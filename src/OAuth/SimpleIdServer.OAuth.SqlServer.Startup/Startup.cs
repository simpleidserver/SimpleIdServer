// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OAuth.EF;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace SimpleIdServer.OAuth.SqlServer.Startup
{
    public class Startup
    {
        public Startup() { }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var json = File.ReadAllText("oauth_key.txt");
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var rsaParameters = new RSAParameters
            {
                Modulus = dic.TryGet(RSAFields.Modulus),
                Exponent = dic.TryGet(RSAFields.Exponent)
            };
            var oauthRsaSecurityKey = new RsaSecurityKey(rsaParameters);
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(o => { });
            services.AddAuthorization(opts => opts.AddDefaultOAUTHAuthorizationPolicy());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddJwtBearer(Constants.AuthenticationScheme, cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                        {
                            "gatewayClient"
                        },
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            services.AddSIDOAuth(o =>
            {
                o.ClientSecretExpirationInSeconds = 2;
            }).AdOAuthEF(opt =>
            {
                opt.UseSqlServer("Data Source=DESKTOP-T4INEAM\\SQLEXPRESS;Initial Catalog=OAuth;Integrated Security=True", o => o.MigrationsAssembly(migrationsAssembly));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            app.UseAuthentication();
            app.UseMvc();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<OAuthDBContext>())
                {
                    context.Database.Migrate();
                    if (context.OAuthClients.Any())
                    {
                        return;
                    }

                    var json = File.ReadAllText("oauth_key.txt");
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    var rsaParameters = new RSAParameters
                    {
                        Modulus = dic.TryGet(RSAFields.Modulus),
                        Exponent = dic.TryGet(RSAFields.Exponent)
                    };

                    SimpleIdServer.Jwt.JsonWebKey sigJsonWebKey;
                    using (var rsa = RSA.Create())
                    {
                        rsa.Import(dic);
                        sigJsonWebKey = new JsonWebKeyBuilder().NewSign("1", new[]
                        {
                            KeyOperations.Sign,
                            KeyOperations.Verify
                        }).SetAlg(rsa, "RS256").Build();
                    }

                    context.OAuthClients.AddRange(DefaultConfiguration.Clients);
                    context.OAuthScopes.AddRange(DefaultConfiguration.Scopes);
                    context.JsonWebKeys.Add(sigJsonWebKey);
                    context.SaveChanges();
                }
            }
        }
    }
}