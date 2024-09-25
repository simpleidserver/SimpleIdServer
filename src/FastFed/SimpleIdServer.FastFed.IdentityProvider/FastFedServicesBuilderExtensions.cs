// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using SimpleIdServer.FastFed;
using SimpleIdServer.FastFed.IdentityProvider;
using SimpleIdServer.FastFed.IdentityProvider.Jobs;
using SimpleIdServer.FastFed.IdentityProvider.Options;
using SimpleIdServer.FastFed.IdentityProvider.Services;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddFastFedIdentityProvider(this FastFedServicesBuilder builder, 
        Action<ProviderStoreChooser> cbChooser = null, 
        Action<FastFedIdentityProviderOptions> callback = null,
        Action<IGlobalConfiguration> hangfireCb = null)
    {
        if (callback == null) builder.Services.Configure<FastFedIdentityProviderOptions>(o => { });
        else builder.Services.Configure(callback);
        if (cbChooser != null) cbChooser(new ProviderStoreChooser(builder.Services));
        builder.Services.AddTransient<IFastFedService, FastFedService>();
        builder.Services.AddTransient<ProvisionRepresentationsJob>();
        builder.Services.AddHangfire(hangfireCb == null ? (o => {
            o.UseRecommendedSerializerSettings();
            o.UseIgnoredAssemblyVersionTypeResolver();
            o.UseInMemoryStorage();
        }) : hangfireCb);
        builder.Services.AddHangfireServer();
        return builder;
    }

    public static FastFedServicesBuilder UseDefaultIdProviderSecurity(this FastFedServicesBuilder builder,
        AuthOptions authOptions = null,
        Action<FastFedIdentityProviderOptions> callback = null)
    {
        authOptions = authOptions ?? new AuthOptions();
        var opts = new FastFedIdentityProviderOptions();
        if (callback != null) callback(opts);
        builder.Services.AddAuthorization(b =>
        {
            b.AddPolicy(DefaultPolicyNames.IsAdminUser, b =>
            {
                b.RequireAuthenticatedUser();
                b.RequireAssertion(c =>
                {
                    return c.User.Claims.Any(c => c.Type == "role" && c.Value == authOptions.AdministratorRole);
                });
            });
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = opts.AuthScheme.Cookie;
            options.DefaultChallengeScheme = opts.AuthScheme.Openid;
        })
            .AddCookie(opts.AuthScheme.Cookie)
            .AddOpenIdConnect(opts.AuthScheme.Openid, options =>
            {
                options.SignInScheme = opts.AuthScheme.Cookie;
                options.ResponseType = "code";
                options.Authority = authOptions.Authority;
                options.RequireHttpsMetadata = false;
                options.ClientId = authOptions.ClientId;
                options.ClientSecret = authOptions.ClientSecret;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("role");
                options.ClaimActions.MapJsonKey("role", "role");
                options.SaveTokens = true;
            });
        return builder;
    }
}
