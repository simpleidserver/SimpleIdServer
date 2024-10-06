// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.FastFed;
using SimpleIdServer.FastFed.ApplicationProvider;
using SimpleIdServer.FastFed.ApplicationProvider.Options;
using SimpleIdServer.FastFed.ApplicationProvider.Resolvers;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using SimpleIdServer.Webfinger.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

public static class FastFedServicesBuilderExtensions
{
    public static FastFedServicesBuilder AddFastFedApplicationProvider(
        this FastFedServicesBuilder builder, 
        Action<ProviderStoreChooser> cbChooser, 
        Action<FastFedApplicationProviderOptions> callback = null, 
        Action<IBusRegistrationConfigurator> massTransitOptions = null)
    {
        if (callback == null) builder.Services.Configure<FastFedApplicationProviderOptions>(o => { });
        else builder.Services.Configure(callback);
        builder.Services.AddTransient<IFastFedService, FastFedService>();
        builder.Services.AddTransient<IWebfingerClientFactory, WebfingerClientFactory>();
        builder.Services.AddTransient<IWebfingerUrlResolver, WebfingerUrlResolver>();
        builder.Services.AddMassTransit(o =>
        {
            if (massTransitOptions != null) massTransitOptions(o);
            else
            {
                o.UsingInMemory();
            }
        });
        cbChooser(new ProviderStoreChooser(builder.Services));
        return builder;
    }

    public static FastFedServicesBuilder UseDefaultAppProviderSecurity(this FastFedServicesBuilder builder,
        AuthOptions authOptions = null,
        Action<FastFedApplicationProviderOptions> callback = null)
    {
        authOptions = authOptions ?? new AuthOptions();
        var opts = new FastFedApplicationProviderOptions();
        if (callback != null) callback(opts);
        builder.Services.AddSingleton(authOptions);
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
            b.AddPolicy(DefaultPolicyNames.IsAdminScope, b =>
            {
                b.AuthenticationSchemes.Add(opts.AuthScheme.Jwt);
                b.RequireClaim("scope", authOptions.AdministratorScope);
            });
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = opts.AuthScheme.Cookie;
            options.DefaultChallengeScheme = opts.AuthScheme.Openid;
        })
            .AddCookie(opts.AuthScheme.Cookie)
            .AddCustomOpenIdConnect(opts.AuthScheme.Openid, options =>
            {
                if (authOptions.IgnoreCertificateError)
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                        {
                            return true;
                        }
                    };
                    options.BackchannelHttpHandler = handler;
                }

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
            })
            .AddJwtBearer(opts.AuthScheme.Jwt, options =>
            {
                options.Authority = authOptions.Authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new List<string>
                    {
                        authOptions.Audience
                    },
                    ValidIssuers = new List<string>
                    {
                        authOptions.Authority
                    }
                };
            });
        return builder;
    }
}
