// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProtectAPIWithClientCredentialsGrantType.Api.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ProtectAPIWithClientCredentialsGrantType.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = ExtractKey("oauth_puk.txt"),
                    ValidAudiences = new List<string>
                    {
                        "application"
                    },
                    ValidIssuers = new List<string>
                    {
                        "https://localhost:60001"
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("GetUser", p => p.RequireClaim("scope", "get_user"));
                options.AddPolicy("AddUser", p => p.RequireClaim("scope", "add_user"));
            });
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "DefaultRoute",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private RsaSecurityKey ExtractKey(string fileName)
        {
            var json = File.ReadAllText(Path.Combine(_env.ContentRootPath, fileName));
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            using (var rsa = RSA.Create())
            {
                var rsaParameters = new RSAParameters
                {
                    Modulus = Convert.FromBase64String(dic["n"].ToString()),
                    Exponent = Convert.FromBase64String(dic["e"].ToString())
                };
                rsa.ImportParameters(rsaParameters);
                return new RsaSecurityKey(rsa);
            }
        }
    }
}