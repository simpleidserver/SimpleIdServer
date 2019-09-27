// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Api.Authorization;
using SimpleIdServer.OpenID.Api.Authorization.ResponseTypes;
using SimpleIdServer.OpenID.Api.Authorization.Validators;
using SimpleIdServer.OpenID.Api.Register;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using SimpleIdServer.OpenID.Persistence.InMemory;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using SimpleIdServer.OpenID.UI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register OPENID dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerOpenIDBuilder AddSIDOpenID(this IServiceCollection services)
        {
            var builder = new SimpleIdServerOpenIDBuilder(services);
            services.AddSIDOAuth();
            services
                .AddUI()
                .AddOpenIDStore()
                .AddOpenIDAuthorizationApi()
                .AddRegisterApi()
                .AddOpenIDAuthentication();
            return builder;
        }

        /// <summary>
        /// Register OPENID dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SimpleIdServerOpenIDBuilder AddSIDOpenID(this IServiceCollection services, Action<OpenIDHostOptions> options)
        {
            services.Configure(options);
            return services.AddSIDOpenID();
        }

        private static IServiceCollection AddOpenIDAuthentication(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var openidHostOptions = serviceProvider.GetService<IOptionsMonitor<OpenIDHostOptions>>();
            services.AddAuthentication(openidHostOptions.CurrentValue.AuthenticationScheme)
                .AddCookie(openidHostOptions.CurrentValue.AuthenticationScheme, openidHostOptions.CurrentValue.AuthenticationScheme, opts =>
                {
                    opts.Events.OnSigningIn += (CookieSigningInContext ctx) =>
                    {
                        var nameIdentifier = ctx.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                        var ticket = new AuthenticationTicket(ctx.Principal, ctx.Properties, ctx.Scheme.Name);
                        var cookieValue = ctx.Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding(ctx));
                        ctx.Options.CookieManager.AppendResponseCookie(
                            ctx.HttpContext,
                            $"{ctx.Options.Cookie.Name}-{nameIdentifier}",
                            cookieValue,
                            ctx.CookieOptions);
                        return Task.CompletedTask;
                    };
                    opts.Events.OnSigningOut += (CookieSigningOutContext ctx) =>
                    {
                        var nameIdentifier = ctx.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                        ctx.Options.CookieManager.DeleteCookie(
                            ctx.HttpContext,
                            $"{ctx.Options.Cookie.Name}-{nameIdentifier}",
                            ctx.CookieOptions);
                        return Task.CompletedTask;
                    };
                });
            return services;
        }

        private static IServiceCollection AddUI(this IServiceCollection services)
        {
            services.AddTransient<ISessionManager, SessionManager>();
            return services;
        }

        private static IServiceCollection AddOpenIDStore(this IServiceCollection services)
        {
            var acrs = new List<AuthenticationContextClassReference>();
            var clients = new List<OpenIdClient>();
            var scopes = new List<OpenIdScope>
            {
                SIDOpenIdConstants.StandardScopes.Address,
                SIDOpenIdConstants.StandardScopes.Email,
                SIDOpenIdConstants.StandardScopes.OfflineAccessScope,
                SIDOpenIdConstants.StandardScopes.OpenIdScope,
                SIDOpenIdConstants.StandardScopes.Phone,
                SIDOpenIdConstants.StandardScopes.Profile
            };
            services.AddSingleton<IAuthenticationContextClassReferenceQueryRepository>(new DefaultAuthenticationContextClassReferenceQueryRepository(acrs));
            services.AddSingleton<IAuthenticationContextClassReferenceCommandRepository>(new DefaultAuthenticationContextClassReferenceCommandRepository(acrs));
            services.AddSingleton<IOAuthClientQueryRepository>(new DefaultOpenIdClientQueryRepository(clients));
            services.AddSingleton<IOAuthClientCommandRepository>(new DefaultOpenIdClientCommandRepository(clients));
            services.AddSingleton<IOAuthScopeQueryRepository>(new DefaultOpenIdScopeQueryRepository(scopes));
            services.AddSingleton<IOAuthScopeCommandRepository>(new DefaultOpenIdScopeCommandRepository(scopes));
            return services;
        }

        private static IServiceCollection AddOpenIDAuthorizationApi(this IServiceCollection services)
        {
            services.RemoveAll<IAuthorizationRequestEnricher>();
            services.RemoveAll<IAuthorizationRequestValidator>();
            services.RemoveAll<IResponseTypeHandler>();
            services.RemoveAll<IAuthorizationRequestHandler>();
            services.RemoveAll<IUserConsentFetcher>();
            services.AddTransient<IUserConsentFetcher, OpenIDUserConsentFetcher>();
            services.AddTransient<IAuthorizationRequestHandler, OpenIDAuthorizationRequestHandler>();
            services.AddTransient<IAuthorizationRequestValidator, OpenIDAuthorizationRequestValidator>();
            services.AddTransient<IResponseTypeHandler, SimpleIdServer.OpenID.Api.Authorization.ResponseTypes.AuthorizationCodeResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, SimpleIdServer.OpenID.Api.Authorization.ResponseTypes.TokenResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, IdTokenResponseTypeHandler>();
            services.AddTransient<ITokenBuilder, IdTokenBuilder>();
            services.AddTransient<IAuthorizationRequestEnricher, OpenIDAuthorizationRequestEnricher>();
            services.AddTransient<ISubjectTypeBuilder, PublicSubjectTypeBuilder>();
            services.AddTransient<ISubjectTypeBuilder, PairWiseSubjectTypeBuidler>();
            services.AddTransient<IAmrHelper, AmrHelper>();
            return services;
        }

        private static IServiceCollection AddRegisterApi(this IServiceCollection services)
        {
            services.RemoveAll<IRegisterRequestHandler>();
            services.AddTransient<IRegisterRequestHandler, OpenIDRegisterRequestHandler>();
            return services;
        }

        private static string GetTlsTokenBinding(CookieSigningInContext context)
        {
            var binding = context.HttpContext.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}