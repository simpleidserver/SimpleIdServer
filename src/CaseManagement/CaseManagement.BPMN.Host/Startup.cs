// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using CaseManagement.BPMN.Domains;
using CaseManagement.BPMN.Host.Delegates;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CaseManagement.BPMN.Host
{
    public class Startup
    {
        private Dictionary<string, string> MAPPING_OPENIDCLAIM_TO_CLAIM = new Dictionary<string, string>
        {
            { "sub", ClaimTypes.NameIdentifier },
            { "role", ClaimTypes.Role }
        };
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment env, IConfiguration configuration) 
        {
            _env = env;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var files = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "Bpmns"), "*.bpmn").ToList();
            services
                .AddMvc(opts => opts.EnableEndpointRouting = false)
                .AddNewtonsoftJson();
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async (ctx) =>
                    {
                        var issuer = ctx.Principal.Claims.First(c => c.Type == "iss").Value;
                        using (var httpClient = new HttpClient())
                        {
                            var authorization = ctx.Request.Headers["Authorization"][0];
                            var bearer = authorization.Split(" ").Last();
                            var requestMessage = new HttpRequestMessage
                            {
                                RequestUri = new Uri($"{issuer}/userinfo"),
                                Method = HttpMethod.Get
                            };
                            requestMessage.Headers.Add("Authorization", $"Bearer {bearer}");
                            var httpResponse = await httpClient.SendAsync(requestMessage);
                            var json = await httpResponse.Content.ReadAsStringAsync();
                            var jObj = JObject.Parse(json);
                            var identity = new ClaimsIdentity("userInfo");
                            foreach (var kvp in jObj)
                            {
                                var key = kvp.Key;
                                if (MAPPING_OPENIDCLAIM_TO_CLAIM.ContainsKey(key))
                                {
                                    key = MAPPING_OPENIDCLAIM_TO_CLAIM[key];
                                }

                                if (kvp.Value.ToString().StartsWith('['))
                                {
                                    var arr = JArray.Parse(kvp.Value.ToString()).Select(_ => _.ToString()).ToList();
                                    foreach (var str in arr)
                                    {
                                        identity.AddClaim(new Claim(kvp.Key, str));
                                    }
                                }
                                else
                                {
                                    identity.AddClaim(new Claim(kvp.Key, kvp.Value.ToString()));
                                }
                            }

                            var principal = new ClaimsPrincipal(identity);
                            ctx.Principal = principal;
                        }
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = ExtractKey("openid_puk.txt"),
                    ValidAudiences = new List<string>
                    {
                        "caseManagementWebsite",
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
            services.AddAuthorization(_ => _.AddDefaultBPMNAuthorizationPolicy());
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddProcessJobServer(callbackServerOpts: opts =>
            {
                opts.WSHumanTaskAPI = "http://localhost:60006";
                opts.CallbackUrl = "http://localhost:60007/processinstances/{id}/complete/{eltId}";
            }).AddProcessFiles(files).AddDelegateConfigurations(GetDelegateConfigurations());
            services.AddSwaggerGen();
            services.AddMassTransitHostedService();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (_configuration.GetChildren().Any(i => i.Key == "pathBase"))
            {
                app.UsePathBase(_configuration["pathBase"]);
            }

            app.UseForwardedHeaders();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BPMN API V1");
            });
            app.UseAuthentication();
            app.UseCors("AllowAll");
            app.UseMvc();
        }

        private RsaSecurityKey ExtractKey(string fileName)
        {
            var json = File.ReadAllText(Path.Combine(_env.ContentRootPath, fileName));
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters
            {
                Modulus = Base64DecodeBytes(dic["n"].ToString()),
                Exponent = Base64DecodeBytes(dic["e"].ToString())
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

        private static ConcurrentBag<DelegateConfigurationAggregate> GetDelegateConfigurations()
        {
            var credential = JsonConvert.DeserializeObject<CredentialsParameter>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "credentials.json")));
            
            var sendEmailDelegate = DelegateConfigurationAggregate.Create("SendEmailDelegate", typeof(SendEmailDelegate).FullName);
            sendEmailDelegate.AddDisplayName("fr", "Envoyer un email");
            sendEmailDelegate.AddDisplayName("en", "Send email");
            sendEmailDelegate.AddRecord("httpBody", "Please update the password by clicking on the website {{configuration.Get('humanTaskUrl')}}/humantaskinstances/{{messages.Get('humanTaskCreated', 'humanTaskInstance.id')}}?auth=email, the OTP code is {{messages.Get('otp', 'otpCode')}}");
            sendEmailDelegate.AddRecord("subject", "Update password");
            sendEmailDelegate.AddRecord("fromEmail", credential.Login);
            sendEmailDelegate.AddRecord("smtpHost", "smtp.gmail.com");
            sendEmailDelegate.AddRecord("smtpPort", "587");
            sendEmailDelegate.AddRecord("smtpUserName", credential.Login);
            sendEmailDelegate.AddRecord("smtpPassword", credential.Password);
            sendEmailDelegate.AddRecord("humanTaskUrl", "http://localhost:4200");
            sendEmailDelegate.AddRecord("smtpEnableSsl", "true");

            var updateUserPasswordDelegate = DelegateConfigurationAggregate.Create("UpdateUserPasswordDelegate", typeof(UpdateUserPasswordDelegate).FullName);
            updateUserPasswordDelegate.AddDisplayName("fr", "Mettre à jour le mot de passe");
            updateUserPasswordDelegate.AddDisplayName("en", "Update password");
            updateUserPasswordDelegate.AddRecord("clientId", "humanTaskClient");
            updateUserPasswordDelegate.AddRecord("clientSecret", "humanTaskClientSecret");
            updateUserPasswordDelegate.AddRecord("tokenUrl", "https://localhost:60000/token");
            updateUserPasswordDelegate.AddRecord("userUrl", "https://localhost:60000/management/users/{id}/password");
            updateUserPasswordDelegate.AddRecord("scope", "manage_users");

            var generateOTPDelegate = DelegateConfigurationAggregate.Create("GenerateOTPDelegate", typeof(GenerateOTPDelegate).FullName);
            generateOTPDelegate.AddDisplayName("fr", "Générer le code OTP");
            generateOTPDelegate.AddDisplayName("en", "Generate OTP code");
            generateOTPDelegate.AddRecord("clientId", "humanTaskClient");
            generateOTPDelegate.AddRecord("clientSecret", "humanTaskClientSecret");
            generateOTPDelegate.AddRecord("tokenUrl", "https://localhost:60000/token");
            generateOTPDelegate.AddRecord("userUrl", "https://localhost:60000/management/users/{id}/otp");
            generateOTPDelegate.AddRecord("scope", "manage_users");

            var assignHumanTaskInstanceDelegate = DelegateConfigurationAggregate.Create("assignHumanTask", typeof(AssignHumanTaskInstanceDelegate).FullName);
            assignHumanTaskInstanceDelegate.AddDisplayName("fr", "Assigner la tâche humaine");
            assignHumanTaskInstanceDelegate.AddDisplayName("en", "Assign human task");
            assignHumanTaskInstanceDelegate.AddRecord("clientId", "humanTaskClient");
            assignHumanTaskInstanceDelegate.AddRecord("clientSecret", "humanTaskClientSecret");
            assignHumanTaskInstanceDelegate.AddRecord("tokenUrl", "https://localhost:60000/token");
            assignHumanTaskInstanceDelegate.AddRecord("humanTaskInstanceClaimUrl", "http://localhost:60006/humantaskinstances/{id}/force/claim");
            assignHumanTaskInstanceDelegate.AddRecord("humanTaskInstanceStartUrl", "http://localhost:60006/humantaskinstances/{id}/force/start");

            return new ConcurrentBag<DelegateConfigurationAggregate>
            {
                sendEmailDelegate,
                updateUserPasswordDelegate,
                generateOTPDelegate,
                assignHumanTaskInstanceDelegate
            };
        }

        private class CredentialsParameter
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }
    }
}