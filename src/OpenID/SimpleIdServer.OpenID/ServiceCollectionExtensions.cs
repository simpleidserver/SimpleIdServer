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
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.Api.Authorization;
using SimpleIdServer.OpenID.Api.Authorization.ResponseTypes;
using SimpleIdServer.OpenID.Api.Authorization.Validators;
using SimpleIdServer.OpenID.Api.Register;
using SimpleIdServer.OpenID.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.Domains.ACRs;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using SimpleIdServer.OpenID.UI.Infrastructures;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenID(this IServiceCollection services)
        {
            return services.AddUI()
                .AddOpenIDStore()
                .AddOpenIDAuthorizationApi()
                .AddRegisterApi()
                .AddOpenIDAuthentication();
        }

        public static IServiceCollection AddOpenIDAuthentication(this IServiceCollection services)
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

        public static IServiceCollection AddUI(this IServiceCollection services)
        {
            services.AddTransient<ISessionManager, SessionManager>();
            return services;
        }

        public static IServiceCollection AddAggregateHttpClaimsSource(this IServiceCollection services, AggregateHttpClaimsSourceOptions httpClaimsSourceOptions)
        {
            services.AddTransient<IClaimsSource, AggregateHttpClaimsSource>(o => new AggregateHttpClaimsSource(httpClaimsSourceOptions, o.GetService<IJwtBuilder>(), o.GetService<IJwtParser>()));
            return services;
        }

        public static IServiceCollection AddDistributeHttpClaimsSource(this IServiceCollection services, DistributeHttpClaimsSourceOptions distributeHttpClaimsSourceOptions)
        {
            services.AddTransient<IClaimsSource, DistributeHttpClaimsSource>(o => new DistributeHttpClaimsSource(distributeHttpClaimsSourceOptions));
            return services;
        }

        public static IServiceCollection AddOpenIDStore(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationContextClassReferenceQueryRepository>(o =>
            {
                var opt = o.GetService<IOptions<OpenIDHostOptions>>();
                return new DefaultAuthenticationContextClassReferenceQueryRepository(opt.Value.DefaultAuthenticationContextClassReferences);
            });
            services.AddSingleton<IAuthenticationContextClassReferenceCommandRepository>(o =>
            {
                var opt = o.GetService<IOptions<OpenIDHostOptions>>();
                return new DefaultAuthenticationContextClassReferenceCommandRepository(opt.Value.DefaultAuthenticationContextClassReferences);
            });
            return services;
        }

        public static IServiceCollection AddOpenIDAuthorizationApi(this IServiceCollection services)
        {
            services.RemoveAll<IAuthorizationRequestEnricher>();
            services.RemoveAll<IAuthorizationRequestValidator>();
            services.RemoveAll<IResponseTypeHandler>();
            services.RemoveAll<IAuthorizationRequestHandler>();
            services.RemoveAll<IUserConsentFetcher>();
            services.AddTransient<IUserConsentFetcher, OpenIDUserConsentFetcher>();
            services.AddTransient<IAuthorizationRequestHandler, OpenIDAuthorizationRequestHandler>();
            services.AddTransient<IAuthorizationRequestValidator, OpenIDAuthorizationRequestValidator>();
            services.AddTransient<IResponseTypeHandler, Api.Authorization.ResponseTypes.AuthorizationCodeResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, Api.Authorization.ResponseTypes.TokenResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, IdTokenResponseTypeHandler>();
            services.AddTransient<ITokenBuilder, IdTokenBuilder>();
            services.AddTransient<IAuthorizationRequestEnricher, OpenIDAuthorizationRequestEnricher>();
            services.AddTransient<ISubjectTypeBuilder, PublicSubjectTypeBuilder>();
            services.AddTransient<ISubjectTypeBuilder, PairWiseSubjectTypeBuidler>();
            services.AddTransient<IAmrHelper, AmrHelper>();
            return services;
        }

        public static IServiceCollection AddRegisterApi(this IServiceCollection services)
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