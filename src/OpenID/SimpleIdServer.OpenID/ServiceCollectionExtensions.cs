// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Register.Handlers;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Api.Token;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Api.Authorization;
using SimpleIdServer.OpenID.Api.Authorization.ResponseTypes;
using SimpleIdServer.OpenID.Api.Authorization.Validators;
using SimpleIdServer.OpenID.Api.BCAuthorize;
using SimpleIdServer.OpenID.Api.BCDeviceRegistration;
using SimpleIdServer.OpenID.Api.Configuration;
using SimpleIdServer.OpenID.Api.Register;
using SimpleIdServer.OpenID.Api.Token;
using SimpleIdServer.OpenID.Api.Token.Handlers;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Infrastructures.Jobs;
using SimpleIdServer.OpenID.Infrastructures.Locks;
using SimpleIdServer.OpenID.Jobs;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using SimpleIdServer.OpenID.Persistence.InMemory;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using SimpleIdServer.OpenID.UI.Infrastructures;
using System;
using System.Collections.Concurrent;
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
                .AddOpenIdConfiguration()
                .AddOpenIDStore()
                .AddOpenIDAuthorizationApi()
                .AddRegisterApi()
                .AddBCAuthorizeApi()
                .AddBCDeviceRegistrationApi()
                .AddOpenIDAuthentication()
                .AddBCAuthorizeJob()
                .AddInMemoryLock();
            return builder;
        }

        /// <summary>
        /// Register OPENID dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SimpleIdServerOpenIDBuilder AddSIDOpenID(this IServiceCollection services, Action<OpenIDHostOptions> openidOptions = null, Action<OAuthHostOptions> oauthOptions = null)
        {
            if (openidOptions != null)
            {
                services.Configure(openidOptions);
            }
            else
            {
                services.Configure<OpenIDHostOptions>((opt) => { });
            }

            if (oauthOptions != null)
            {
                services.Configure(oauthOptions);
            }
            else
            {
                services.Configure<OAuthHostOptions>((opt) => { });
            }

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

        private static IServiceCollection AddOpenIdConfiguration(this IServiceCollection services)
        {
            services.RemoveAll<IOAuthWorkflowConverter>();
            services.AddTransient<IOAuthWorkflowConverter, OpenIdWorkflowConverter>();
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
            var bcAuthorizeLst = new ConcurrentBag<BCAuthorize>();
            services.AddSingleton<IAuthenticationContextClassReferenceQueryRepository>(new DefaultAuthenticationContextClassReferenceQueryRepository(acrs));
            services.AddSingleton<IAuthenticationContextClassReferenceCommandRepository>(new DefaultAuthenticationContextClassReferenceCommandRepository(acrs));
            services.AddSingleton<IOAuthClientQueryRepository>(new DefaultOpenIdClientQueryRepository(clients));
            services.AddSingleton<IOAuthClientCommandRepository>(new DefaultOpenIdClientCommandRepository(clients, scopes));
            services.AddSingleton<IOAuthScopeQueryRepository>(new DefaultOpenIdScopeQueryRepository(scopes));
            services.AddSingleton<IOAuthScopeCommandRepository>(new DefaultOpenIdScopeCommandRepository(scopes));
            services.AddSingleton<IBCAuthorizeRepository>(new DefaultBCAuthorizeRepository(bcAuthorizeLst));
            return services;
        }

        private static IServiceCollection AddOpenIDAuthorizationApi(this IServiceCollection services)
        {
            services.RemoveAll<IAuthorizationRequestEnricher>();
            services.RemoveAll<IAuthorizationRequestValidator>();
            services.RemoveAll<IResponseTypeHandler>();
            services.RemoveAll<IAuthorizationRequestHandler>();
            services.RemoveAll<IUserConsentFetcher>();
            services.RemoveAll<ITokenBuilder>();
            services.AddTransient<IUserConsentFetcher, OpenIDUserConsentFetcher>();
            services.AddTransient<IAuthorizationRequestHandler, OpenIDAuthorizationRequestHandler>();
            services.AddTransient<IAuthorizationRequestValidator, OpenIDAuthorizationRequestValidator>();
            services.AddTransient<IResponseTypeHandler, SimpleIdServer.OpenID.Api.Authorization.ResponseTypes.AuthorizationCodeResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, SimpleIdServer.OpenID.Api.Authorization.ResponseTypes.TokenResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, IdTokenResponseTypeHandler>();
            services.AddTransient<ITokenBuilder, IdTokenBuilder>();
            services.AddTransient<ITokenBuilder, OpenIDAccessTokenBuilder>();
            services.AddTransient<ITokenBuilder, OpenIDRefreshTokenBuilder>();
            services.AddTransient<IClaimsJwsPayloadEnricher, ClaimsJwsPayloadEnricher>();
            services.AddTransient<IAuthorizationRequestEnricher, OpenIDAuthorizationRequestEnricher>();
            services.AddTransient<ISubjectTypeBuilder, PublicSubjectTypeBuilder>();
            services.AddTransient<ISubjectTypeBuilder, PairWiseSubjectTypeBuidler>();
            services.AddTransient<IAmrHelper, AmrHelper>();
            services.AddTransient<IExtractRequestHelper, ExtractRequestHelper>();
            return services;
        }

        private static IServiceCollection AddBCAuthorizeApi(this IServiceCollection services)
        {
            services.AddTransient<IGrantTypeHandler, CIBAHandler>();
            services.AddTransient<ICIBAGrantTypeValidator, CIBAGrantTypeValidator>();
            services.AddTransient<IBCAuthorizeHandler, BCAuthorizeHandler>();
            services.AddTransient<IBCAuthorizeRequestValidator, BCAuthorizeRequestValidator>();
            services.AddTransient<IBCNotificationService, BCNotificationService>();
            return services;
        }

        private static IServiceCollection AddBCDeviceRegistrationApi(this IServiceCollection services)
        {
            services.AddTransient<IBCDeviceRegistrationHandler, BCDeviceRegistrationHandler>();
            services.AddTransient<IBCDeviceRegistrationValidator, BCDeviceRegistrationValidator>();
            return services;
        }

        private static IServiceCollection AddBCAuthorizeJob(this IServiceCollection services)
        {
            services.AddHostedService<OpenIdServerHostedService>();
            services.AddTransient<IOpenIdJobServer, OpenIdJobServer>();
            services.AddTransient<IBCNotificationHandler, PingNotificationHandler>();
            services.AddTransient<IBCNotificationHandler, PushNotificationHandler>();
            services.AddTransient<IJob, BCNotificationJob>();
            return services;
        }

        private static IServiceCollection AddInMemoryLock(this IServiceCollection services)
        {
            services.AddSingleton<IDistributedLock, InMemoryDistributedLock>();
            return services;
        }

        private static IServiceCollection AddRegisterApi(this IServiceCollection services)
        {
            services.RemoveAll<IAddOAuthClientHandler>();
            services.RemoveAll<IGetOAuthClientHandler>();
            services.RemoveAll<IUpdateOAuthClientHandler>();
            services.RemoveAll<IOAuthClientValidator>();
            services.AddTransient<IAddOAuthClientHandler, AddOpenIdClientHandler>();
            services.AddTransient<IGetOAuthClientHandler, GetOpenIdClientHandler>();
            services.AddTransient<IUpdateOAuthClientHandler, UpdateOpenIdClientHandler>();
            services.AddTransient<IOAuthClientValidator, OpenIdClientValidator>();
            return services;
        }

        private static string GetTlsTokenBinding(CookieSigningInContext context)
        {
            var binding = context.HttpContext.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}