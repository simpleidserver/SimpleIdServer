// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UseUMAToProtectAPI.Api.Persistence;

namespace UseUMAToProtectAPI.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = ExtractKey("oauth_puk.txt"),
                        ValidAudiences = new List<string>
                        {
                            "portal"
                        },
                        ValidIssuers = new List<string>
                        {
                            "https://localhost:60001"
                        }
                    };
                })
                .AddJwtBearer("OpenIdAuthenticationScheme", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = ExtractKey("openid_puk.txt"),
                        ValidAudiences = new List<string>
                        {
                            "https://localhost:60000"
                        },
                        ValidIssuers = new List<string>
                        {
                            "https://localhost:60000"
                        }
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", builder =>
                {
                    builder.AddAuthenticationSchemes("OpenIdAuthenticationScheme");
                    builder.RequireAuthenticatedUser();
                });
            });
            services.AddMvc(o => o.EnableEndpointRouting = false)
                .AddNewtonsoftJson(o => { });
            services.AddSingleton<IPictureRepository, PictureRepository>();
        }

        public void Configure(IApplicationBuilder app)
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
            var rsa = RSA.Create();
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