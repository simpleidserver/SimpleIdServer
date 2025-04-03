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
using SimpleIdServer.OpenIdConnect;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IdserverAdminBuilder AddIdserverAdmin(this IServiceCollection services, Action<IdServerWebsiteOptions>? callbackOptions = null, string issuer)
    {
        var opts = new IdServerWebsiteOptions
        {
            Issuer = issuer,
        };
        if (callbackOptions != null) callbackOptions(opts);
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddFluxor(o =>
        {
            o.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly);
            o.UseReduxDevTools(rdt =>
            {
                rdt.Name = "SimpleIdServer";
            });
        });
        services.AddLocalization();
        services.RemoveAll<IUriProvider>();
        services.AddFormBuilderUi();
        services.AddHttpContextAccessor();
        services.AddTransient<IUserSessionResitory, UserSessionRepository>();
        services.AddTransient<IUrlHelper, UrlHelper>();
        services.AddTransient<IUriProvider, SidAdmUriProvider>();
        services.AddScoped<IOTPQRCodeGenerator, OTPQRCodeGenerator>();
        services.AddScoped<IGroupService, GroupEffects>();
        services.AddScoped<IRealmStore, CookieRealmStore>();
        services.AddScoped<IWebsiteHttpClientFactory, WebsiteHttpClientFactory>();
        services.AddScoped<DialogService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<ContextMenuService>();
        services.AddScoped<TooltipService>();
        services.AddSingleton<IAccessTokenStore, AccessTokenStore>();
        if (callbackOptions == null) services.Configure<IdServerWebsiteOptions>((o) => { });
        else services.Configure(callbackOptions);
        var b = services.AddDataProtection();
        var adminOpenidAuth = new AdminOpenidAuth();
        var adminAuthz = new AdminAuthz();
        var adminCookieAuth = new AdminCookieAuth();
        ConfigureSecurity(services, adminOpenidAuth, adminAuthz, adminCookieAuth, issuer);
        var result = new IdserverAdminBuilder(services, b, adminOpenidAuth, adminCookieAuth, adminAuthz);
        return result;
    }

    private static IServiceCollection ConfigureSecurity(IServiceCollection services, AdminOpenidAuth adminOpenidAuth, AdminAuthz adminAuthz, AdminCookieAuth adminCookieAuth, string issuer)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "oidc";
        })
        .AddIdServerCookie(CookieAuthenticationDefaults.AuthenticationScheme, null, options =>
        {
            adminCookieAuth.Callback(options);
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = true;
        })
        .AddCustomOpenIdConnect("oidc", config =>
        {
            adminOpenidAuth.Callback(config);
            config.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
            config.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            config.Authority = issuer;
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
        });

        services.AddAuthorization(config =>
        {
            config.AddPolicy("Authenticated", p =>
            {
                p.RequireAuthenticatedUser();
                adminAuthz.Callback(p);
            });
        });

        return services;
    }
}

internal class AdminOpenidAuth
{
    public AdminOpenidAuth()
    {
        Scopes = new List<string> { "openid", "profile", "role" };
        ClientId = "SIDS-manager";
        ClientSecret = "password";
        Callback = (o) =>
        {
            if (IgnoreCertificateError)
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    }
                };
                o.BackchannelHttpHandler = handler;
            }

            o.IsRealmEnabled = UseRealm;
            o.ClientId = ClientId;
            o.ClientSecret = ClientSecret;
            o.Scope.Clear();
            if (Scopes != null)
            {
                foreach (string scope in Scopes) o.Scope.Add(scope);
            }
        };
    }

    internal Action<CustomOpenIdConnectOptions> Callback { get; private set; } 

    internal List<string> Scopes { get; set; }

    internal string ClientId { get; set; }

    internal string ClientSecret { get; set; }

    internal bool IgnoreCertificateError { get; set; }

    internal bool UseRealm { get; set; }
}

internal class AdminCookieAuth
{
    internal AdminCookieAuth()
    {
        CookieName = CookieAuthenticationDefaults.CookiePrefix + Uri.EscapeDataString("AdminWebsite");
        Callback = (o) =>
        {
            o.Cookie.Name = CookieName;
        };
    }

    internal Action<CookieAuthenticationOptions> Callback { get; private set; }

    internal string CookieName { get; set; }
}

internal class AdminAuthz
{
    internal AdminAuthz()
    {
        Callback = (o) =>
        {
            if(RequiredRoles != null)
            {
                foreach(var requiredRole in RequiredRoles)
                {
                    o.RequireRole(requiredRole);
                }
            }
        };
    }

    internal List<string> RequiredRoles { get; set; }

    internal Action<AuthorizationPolicyBuilder> Callback { get; private set; }
}
