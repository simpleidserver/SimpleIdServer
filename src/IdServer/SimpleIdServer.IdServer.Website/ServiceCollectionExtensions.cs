// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.Website;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSIDWebsite(this IServiceCollection services, Action<IdServerWebsiteOptions>? callbackOptions = null, Action<DbContextOptionsBuilder>? action = null)
        {
            services.AddFluxor(o =>
            {
                o.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly);
                o.UseReduxDevTools(rdt =>
                {
                    rdt.Name = "SimpleIdServer";
                });
            });
            services.AddHttpContextAccessor();
            services.AddStoreWithFactory(action);
            services.AddScoped<IOTPQRCodeGenerator, OTPQRCodeGenerator>();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ContextMenuService>();
            services.AddScoped<TooltipService>();
            services.AddSingleton<IWebsiteHttpClientFactory, WebsiteHttpClientFactory>();
            services.AddDIDEthr();
            services.AddTransient<ICertificateAuthorityStore, CertificateAuthorityStore>();
            if (callbackOptions == null) services.Configure<IdServerWebsiteOptions>((o) => { });
            else services.Configure(callbackOptions);
            return services;
        }

        public static IServiceCollection AddDefaultSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            string authoritySectionName = nameof(IdServerWebsiteOptions.IdServerBaseUrl);
            string defaultSecurityOptionsSectionName = nameof(DefaultSecurityOptions);
            string? authority = configuration.GetValue<string>(authoritySectionName);
            if (string.IsNullOrEmpty(authority))
            {
                Console.WriteLine($"Please configure the '{authoritySectionName}' section.");
                Environment.Exit(1);
            }

            DefaultSecurityOptions? defaultSecurityOptions = configuration.GetSection(defaultSecurityOptionsSectionName).Get<DefaultSecurityOptions>();
            if (defaultSecurityOptions == null)
            {
                Console.WriteLine($"Please configure the '{defaultSecurityOptionsSectionName}' section.");
                Environment.Exit(1);
            }

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
            })
            .AddOpenIdConnect("oidc", config =>
            {
                if(defaultSecurityOptions.IgnoreCertificateError)
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                        {
                            return true;
                        }
                    };
                    config.BackchannelHttpHandler = handler;
                }

                config.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
                config.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                config.Authority = defaultSecurityOptions.Issuer;
                config.ClientId = defaultSecurityOptions.ClientId;
                config.ClientSecret = defaultSecurityOptions.ClientSecret;
                config.ResponseType = "code";
                config.ResponseMode = "query";
                config.SaveTokens = true;
                config.GetClaimsFromUserInfoEndpoint = true;
                config.RequireHttpsMetadata = false;
                config.MapInboundClaims = false;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                config.Scope.Clear();
                if (!string.IsNullOrEmpty(defaultSecurityOptions.Scope))
                {
                    string[] scopes = defaultSecurityOptions.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
                    foreach (string scope in scopes) config.Scope.Add(scope);
                }
            });

            services.AddAuthorization(config =>
            {
                var policyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();

                if (!string.IsNullOrEmpty(defaultSecurityOptions.RequiredRole))
                {
                    string[] roles = defaultSecurityOptions.RequiredRole
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
                    policyBuilder.RequireRole(roles);
                }

                config.FallbackPolicy = policyBuilder.Build();
            });

            services.AddSingleton(defaultSecurityOptions);
            return services;
        }
    }
}
