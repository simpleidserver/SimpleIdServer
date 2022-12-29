// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseModes;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Jwks;
using SimpleIdServer.OAuth.Api.Token;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Api.TokenIntrospection;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Authenticate.AssertionParsers;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Stores;
using SimpleIdServer.OAuth.UI;
using SimpleIdServer.Store;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register OAUTH2.0 dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IdServerBuilder AddSIDIdentityServer(this IServiceCollection services, Action<OAuthHostOptions> callback = null)
        {
            if (callback != null) services.Configure(callback);
            else services.Configure<OAuthHostOptions>(o => { });
            services.Configure<RouteOptions>(opt =>
            {
                opt.ConstraintMap.Add("realmPrefix", typeof(RealmRoutePrefixConstraint));
            });
            services.AddControllers();
            services.AddDataProtection();
            services.AddDistributedMemoryCache();
            services.AddStore()
                .AddResponseModeHandlers()
                .AddOAuthClientAuthentication()
                .AddClientAssertionParsers()
                .AddOAuthJwksApi()
                .AddOAuthTokenApi()
                .AddOAuthAuthorizationApi()
                .AddOAuthJwt()
                .AddLib()
                .AddConfigurationApi()
                .AddUI()
                .AddOAuthIntrospectionTokenApi();
            var authBuilder = services.AddAuthentication();
            return new IdServerBuilder(services, authBuilder, services.BuildServiceProvider());
        }

        #region Private methods

        private static IServiceCollection AddResponseModeHandlers(this IServiceCollection services)
        {
            services.AddTransient<IOAuthResponseMode, QueryResponseModeHandler>();
            services.AddTransient<IOAuthResponseMode, FragmentResponseModeHandler>();
            services.AddTransient<IOAuthResponseMode, FormPostResponseModeHandler>();
            services.AddTransient<IOAuthResponseModeHandler, QueryResponseModeHandler>();
            services.AddTransient<IOAuthResponseModeHandler, FragmentResponseModeHandler>();
            services.AddTransient<IOAuthResponseModeHandler, FormPostResponseModeHandler>();
            services.AddTransient<IResponseModeHandler, ResponseModeHandler>();
            return services;
        }

        private static IServiceCollection AddOAuthClientAuthentication(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticateClient>(s => new AuthenticateClient(s.GetService<IClientRepository>(), s.GetServices<IOAuthClientAuthenticationHandler>(), s.GetServices<IClientAssertionParser>(), s.GetService<IOptions<OAuthHostOptions>>() ));
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientPrivateKeyJwtAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretBasicAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretJwtAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretPostAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthPKCEAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientTlsClientAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSelfSignedTlsClientAuthenticationHandler>();
            return services;
        }

        private static IServiceCollection AddClientAssertionParsers(this IServiceCollection services)
        {
            services.AddTransient<IClientAssertionParser, ClientJwtAssertionParser>();
            return services;
        }

        private static IServiceCollection AddOAuthJwksApi(this IServiceCollection services)
        {
            services.AddTransient<IJwksRequestHandler, JwksRequestHandler>();
            services.AddSingleton<IKeyStore, InMemoryKeyStore>();
            return services;
        }

        private static IServiceCollection AddOAuthTokenApi(this IServiceCollection services)
        {
            services.AddTransient<ITokenRequestHandler>(s => new TokenRequestHandler(s.GetServices<IGrantTypeHandler>()));
            services.AddTransient<IClientCredentialsGrantTypeValidator, ClientCredentialsGrantTypeValidator>();
            services.AddTransient<IRefreshTokenGrantTypeValidator, RefreshTokenGrantTypeValidator>();
            services.AddTransient<IAuthorizationCodeGrantTypeValidator, AuthorizationCodeGrantTypeValidator>();
            services.AddTransient<IRevokeTokenValidator, RevokeTokenValidator>();
            services.AddTransient<IPasswordGrantTypeValidator, PasswordGrantTypeValidator>();
            services.AddTransient<IGrantedTokenHelper, GrantedTokenHelper>();
            services.AddTransient<IGrantTypeHandler, ClientCredentialsHandler>();
            services.AddTransient<IGrantTypeHandler, RefreshTokenHandler>();
            services.AddTransient<IGrantTypeHandler, PasswordHandler>();
            services.AddTransient<IGrantTypeHandler, AuthorizationCodeHandler>();
            services.AddTransient<IClientAuthenticationHelper, ClientAuthenticationHelper>();
            services.AddTransient<IRevokeTokenRequestHandler, RevokeTokenRequestHandler>();
            services.AddTransient<ITokenProfile, BearerTokenProfile>();
            services.AddTransient<ITokenProfile, MacTokenProfile>();
            services.AddTransient<ITokenBuilder, AccessTokenBuilder>();
            services.AddTransient<ITokenBuilder, RefreshTokenBuilder>();
            services.AddTransient<ICodeChallengeMethodHandler, PlainCodeChallengeMethodHandler>();
            services.AddTransient<ICodeChallengeMethodHandler, S256CodeChallengeMethodHandler>();
            services.AddTransient<IRequestObjectValidator, RequestObjectValidator>();
            services.AddTransient<IClientHelper, OAuthClientHelper>();
            return services;
        }

        private static IServiceCollection AddOAuthIntrospectionTokenApi(this IServiceCollection services)
        {
            services.AddTransient<ITokenIntrospectionRequestHandler, TokenIntrospectionRequestHandler>();
            return services;
        }

        private static IServiceCollection AddOAuthAuthorizationApi(this IServiceCollection services)
        {
            services.AddTransient<IAuthorizationRequestHandler, AuthorizationRequestHandler>();
            services.AddTransient<IResponseTypeHandler, AuthorizationCodeResponseTypeHandler>();
            services.AddTransient<IResponseTypeHandler, TokenResponseTypeHandler>();
            services.AddTransient<IAuthorizationRequestValidator, OAuthAuthorizationRequestValidator>();
            services.AddTransient<IAuthorizationRequestEnricher, AuthorizationRequestEnricher>();
            services.AddTransient<IUserConsentFetcher, OAuthUserConsentFetcher>();
            return services;
        }

        private static IServiceCollection AddOAuthJwt(this IServiceCollection services)
        {
            services.AddTransient<IJwtBuilder, JwtBuilder>();
            return services;
        }

        private static IServiceCollection AddLib(this IServiceCollection services)
        {
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            return services;
        }
        
        private static IServiceCollection AddConfigurationApi(this IServiceCollection services)
        {
            services.AddTransient<IConfigurationRequestHandler, ConfigurationRequestHandler>();
            services.AddTransient<IOAuthWorkflowConverter, OAuthWorkflowConverter>();
            return services;
        }

        private static IServiceCollection AddUI(this IServiceCollection services)
        {
            services.AddTransient<IOTPAuthenticator, HOTPAuthenticator>();
            services.AddTransient<IOTPAuthenticator, TOTPAuthenticator>();
            return services;
        }

        #endregion
    }

    public class RealmRoutePrefixConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection) => true;
    }
}