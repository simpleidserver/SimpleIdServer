// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenID.Startup
{
    public class OpenIdStartup
    {
        private readonly IConfiguration _configuration;

        public OpenIdStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var issuerSigningKey = ExtractIssuerSigningKey("openid_key.txt");
            var sigJsonWebKey = ExtractJsonWebKeyFromRSA("openid_key.txt", "RS256");
            var firstMtlsClientJsonWebKey = ExtractJsonWebKeyFromRSA("first_mtlsClient_key.txt", "PS256");
            var secondMtlsClientJsonWebKey = ExtractJsonWebKeyFromRSA("second_mtlsClient_key.txt", "PS256");
            var json = firstMtlsClientJsonWebKey.Serialize().ToString();
            var jObj = secondMtlsClientJsonWebKey.Serialize();
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddAuthorization(opts => opts.AddDefaultOAUTHAuthorizationPolicy());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddJwtBearer(OAuth.Constants.AuthenticationScheme, cfg =>
                {
                    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                        {
                            "gatewayClient"
                        },
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = issuerSigningKey
                    };
                })
                .AddCertificate(o =>
                {
                    o.RevocationFlag = X509RevocationFlag.EntireChain;
                    o.RevocationMode = X509RevocationMode.NoCheck;
                });
            services.AddCommonSID();
            services.AddSIDOpenID(opt =>
                {
                    opt.IsLocalhostAllowed = true;
                    opt.IsRedirectionUrlHTTPSRequired = false;
                    opt.IsInitiateLoginUriHTTPSRequired = true;
                }, opt =>
                {
                    opt.MtlsEnabled = true;
                    opt.DefaultScopes = new List<string>
                    {
                        SIDOpenIdConstants.StandardScopes.Profile.Name,
                        SIDOpenIdConstants.StandardScopes.Email.Name,
                        SIDOpenIdConstants.StandardScopes.Address.Name,
                        SIDOpenIdConstants.StandardScopes.Phone.Name,
                        SIDOpenIdConstants.StandardScopes.OfflineAccessScope.Name
                    };
                })
                .AddClients(OpenIdDefaultConfiguration.GetClients(firstMtlsClientJsonWebKey, secondMtlsClientJsonWebKey, sigJsonWebKey), OpenIdDefaultConfiguration.Scopes)
                .AddAcrs(OpenIdDefaultConfiguration.AcrLst)
                .AddUsers(OpenIdDefaultConfiguration.Users)
                .AddJsonWebKeys(new List<JsonWebKey> { sigJsonWebKey })
                .AddLoginPasswordAuthentication()
                .AddSMSAuthentication()
                .AddDynamicAuthenticationProviders()
                .AddAuthenticationProviderSchemes(OpenIdDefaultConfiguration.AuthenticationProviderSchemes);
            // ConfigureFireBase();
            var d = Directory.GetCurrentDirectory();
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory()));
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

        private void ConfigureFireBase()
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
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

        private static Microsoft.IdentityModel.Tokens.RsaSecurityKey ExtractIssuerSigningKey(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var rsaParameter = new RSAParameters
            {
                Modulus = dic.TryGet(RSAFields.Modulus),
                Exponent = dic.TryGet(RSAFields.Exponent)
            };
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsaParameter);
        }
    }
}