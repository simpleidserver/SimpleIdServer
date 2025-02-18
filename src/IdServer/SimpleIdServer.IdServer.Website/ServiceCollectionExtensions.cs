// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.Website;
using SimpleIdServer.IdServer.Website.Helpers;
using SimpleIdServer.IdServer.Website.Infrastructures;
using SimpleIdServer.IdServer.Website.Stores.GroupStore;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSIDWebsite(this IServiceCollection services, Action<IdServerWebsiteOptions>? callbackOptions = null, Action<IDataProtectionBuilder>? dataProtectionBuilderCallback = null)
        {
            var opts = new IdServerWebsiteOptions();
            if (callbackOptions != null) callbackOptions(opts);
            services.AddFluxor(o =>
            {
                o.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly);
                o.UseReduxDevTools(rdt =>
                {
                    rdt.Name = "SimpleIdServer";
                });
            });
            services.RemoveAll<IUriProvider>();
            services.AddFormBuilderUi();
            services.AddHttpContextAccessor();
            services.AddTransient<IUserSessionResitory, UserSessionRepository>();
            services.AddTransient<IUrlHelper, UrlHelper>();
            services.AddTransient<IUriProvider, SidAdmUriProvider>();
            services.AddScoped<IOTPQRCodeGenerator, OTPQRCodeGenerator>();
            services.AddScoped<IGroupService, GroupEffects>();
            services.AddScoped<IRealmStore, RealmStore>();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ContextMenuService>();
            services.AddScoped<TooltipService>();
            services.AddSingleton<IWebsiteHttpClientFactory, WebsiteHttpClientFactory>();
            if (callbackOptions == null) services.Configure<IdServerWebsiteOptions>((o) => { });
            else services.Configure(callbackOptions);
            var b = services.AddDataProtection();
            if (dataProtectionBuilderCallback != null) dataProtectionBuilderCallback(b);
            return services;
        }

        public static IServiceCollection AddDefaultSecurity(this IServiceCollection services, IConfiguration configuration, bool isRealmEnabled, string cookieName = null)
        {
            cookieName = cookieName ?? CookieAuthenticationDefaults.CookiePrefix + Uri.EscapeDataString("AdminWebsite");
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
            .AddIdServerCookie(CookieAuthenticationDefaults.AuthenticationScheme, null, options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = cookieName;
            })
            .AddCustomOpenIdConnect("oidc", config =>
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

                config.IsRealmEnabled = isRealmEnabled;
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
                config.ClaimActions.MapJsonKey(ClaimTypes.Role, "role");
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
