// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SimpleIdServer.Gateway.Host.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleIdServer.Gateway.Host
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration) 
        {
            Env = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment Env { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddAuthentication()
            .AddJwtBearer("Authenticated", options =>
            {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     IssuerSigningKey = ExtractKey("openid_puk.txt"),
                     ValidAudiences = new List<string>
                     {
                        "https://localhost:60000",
                        "https://simpleidserver.northeurope.cloudapp.azure.com/openid"
                     },
                     ValidIssuers = new List<string>
                     {
                        "https://localhost:60000",
                        "https://simpleidserver.northeurope.cloudapp.azure.com/openid"
                     }
                 };
            });
            services.AddLogging();
            services.AddOcelot(Configuration)
                .AddDelegatingHandler<ManageClientClientCredentialsHandler>();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Configuration.GetChildren().Any(i => i.Key == "pathBase"))
            {
                app.UsePathBase(Configuration["pathBase"]);
            }

            app.UseForwardedHeaders();
            app.UseCors("AllowAll");
            app.UseOcelot().Wait();
        }

        private RsaSecurityKey ExtractKey(string fileName)
        {
            var json = File.ReadAllText(Path.Combine(Env.ContentRootPath, fileName));
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters
            {
                Modulus = Base64DecodeBytes(dic["n"].ToString()),
                Exponent = Base64DecodeBytes(dic["e"].ToString()),
            };
            rsa.ImportParameters(rsaParameters);
            return new RsaSecurityKey(rsa);
        }

        private static byte[] Base64DecodeBytes(string base64EncodedData)
        {
            var s = base64EncodedData
                .Trim()
                .Replace(" ", "+")
                .Replace('-', '+')
                .Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0:
                    return Convert.FromBase64String(s);
                case 2:
                    s += "==";
                    goto case 0;
                case 3:
                    s += "=";
                    goto case 0;
                default:
                    throw new InvalidOperationException("Illegal base64url string!");
            }
        }
    }
}