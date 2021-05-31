// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OpenBankingApi.Infrastructure.Filters;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenID;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.OpenBankingApi.EF.Startup
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
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddMvc(option =>
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddCertificate(o =>
                {
                    o.AllowedCertificateTypes = CertificateTypes.All;
                    o.RevocationFlag = X509RevocationFlag.EntireChain;
                    o.RevocationMode = X509RevocationMode.NoCheck;
                });
            services.AddAuthorization(opts =>
            {
                opts.AddDefaultOAUTHAuthorizationPolicy();
                opts.AddOpenBankingAuthorization(CertificateAuthenticationDefaults.AuthenticationScheme);
            });

            services.AddMediatR(typeof(IAccountRepository));
            services.AddSwaggerGen();
            services.AddSIDOpenID(opt =>
            {
                opt.IsLocalhostAllowed = true;
                opt.IsRedirectionUrlHTTPSRequired = false;
                opt.IsInitiateLoginUriHTTPSRequired = true;
                opt.DefaultAcrValue = "urn:openbanking:psd2:ca";
            }, opt =>
            {
                opt.MtlsEnabled = true;
                opt.DefaultScopes = new List<string>
                    {
                        SIDOpenIdConstants.StandardScopes.OpenIdScope.Name,
                        SIDOpenIdConstants.StandardScopes.Profile.Name,
                        SIDOpenIdConstants.StandardScopes.Email.Name,
                        SIDOpenIdConstants.StandardScopes.Address.Name,
                        SIDOpenIdConstants.StandardScopes.Phone.Name,
                        "accounts"
                    };
                opt.DefaultTokenSignedResponseAlg = PS256SignHandler.ALG_NAME;
            })
                .AddLoginPasswordAuthentication();
            services.AddOpenBankingApi()
                .AddOpenBankingEF(opt =>
                {
                    opt.UseSqlServer("Data Source=DESKTOP-T4INEAM\\SQLEXPRESS;Initial Catalog=OpenBanking;Integrated Security=True", o => o.MigrationsAssembly(migrationsAssembly));
                });
                // .AddAccounts(DefaultConfiguration.Accounts);
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
            app.UseAuthorization();
            app.UseMiddleware<CertificateForwardingMiddleware>();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBanking V1");
            });
            app.UseOpenBankingAPI();
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
            ConfigureFireBase();
        }

        private static JsonWebKey ExtractJsonWebKeyFromRSA(string fileName, string algName, string kid)
        {
            using (var rsa = RSA.Create())
            {
                var json = File.ReadAllText(fileName);
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                rsa.Import(dic);
                return new JsonWebKeyBuilder().NewSign(kid, new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, algName).Build();
            }
        }

        private static List<JsonWebKey> GenerateJsonWebKeys()
        {
            JsonWebKey sigJsonWebKey;
            JsonWebKey encJsonWebKey;
            using (var rsa = RSA.Create())
            {
                sigJsonWebKey = new JsonWebKeyBuilder().NewSign("3", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, "PS256").Build();
            }

            using (var rsa = RSA.Create())
            {
                encJsonWebKey = new JsonWebKeyBuilder().NewEnc("4", new[]
                {
                    KeyOperations.Encrypt,
                    KeyOperations.Decrypt
                }).SetAlg(rsa, RSAOAEPCEKHandler.ALG_NAME).Build();
            }

            return new List<JsonWebKey>
            {
                sigJsonWebKey,
                encJsonWebKey
            };
        }

        private void ConfigureFireBase()
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<OpenBankingDbContext>())
                {
                    context.Database.Migrate();
                    if (context.Accounts.Any())
                    {
                        return;
                    }

                    var firstMtlsClientJsonWebKey = ExtractJsonWebKeyFromRSA("first_mtlsClient_key.txt", "PS256", "1");
                    var secondMtlsClientJsonWebKey = ExtractJsonWebKeyFromRSA("second_mtlsClient_key.txt", "PS256", "2");
                    context.OpenIdClients.AddRange(DefaultConfiguration.GetClients(firstMtlsClientJsonWebKey, secondMtlsClientJsonWebKey));
                    context.OAuthScopes.AddRange(DefaultConfiguration.Scopes);
                    context.Acrs.AddRange(DefaultConfiguration.AcrLst);
                    context.Users.AddRange(DefaultConfiguration.Users);
                    context.JsonWebKeys.AddRange(GenerateJsonWebKeys());
                    context.Accounts.AddRange(DefaultConfiguration.Accounts);
                    context.SaveChanges();
                }
            }
        }

        private static void ExtractCertificate(string path, string pass)
        {
            var certificate = new X509Certificate2(path, pass, X509KeyStorageFlags.Exportable);
            var publicKey = Convert.ToBase64String(certificate.GetRawCertData());
            var privtateKey = Convert.ToBase64String((certificate.GetRSAPrivateKey() as RSACng).ExportRSAPrivateKey());
        }

        private class CertificateForwardingMiddleware
        {
            private readonly RequestDelegate _next;

            public CertificateForwardingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                const string headerName = "X-ARR-ClientCert";
                if (context.Request.Headers.ContainsKey(headerName))
                {
                    var header = HttpUtility.UrlDecode(context.Request.Headers[headerName].First());
                    header = header.Replace("-----BEGIN CERTIFICATE-----", null)
                        .Replace("-----END CERTIFICATE-----", null);
                    context.Connection.ClientCertificate = new X509Certificate2(Convert.FromBase64String(header));
                }

                await _next(context);
            }
        }
    }
}