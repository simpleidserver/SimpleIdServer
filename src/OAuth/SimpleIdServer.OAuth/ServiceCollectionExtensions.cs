// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseModes;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Authorization.Validators;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Jwks;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.OAuth.Api.Token;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Jwks;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;

namespace SimpleIdServer.OAuth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOauth(this IServiceCollection services)
        {
            services.AddOAuthStore()
                .AddResponseModeHandlers()
                .AddOAuthClientAuthentication()
                .AddOAuthJwksApi()
                .AddOAuthTokenApi()
                .AddOAuthAuthorizationApi()
                .AddOAuthJwt()
                .AddLib()
                .AddOAuthService()
                .AddJwt()
                .AddRegisterApi()
                .AddConfigurationApi();
            services.AddDataProtection();
            services.AddAuthorization(policy =>
            {
                policy.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
            });
            return services;
        }

        private static IServiceCollection AddResponseModeHandlers(this IServiceCollection services)
        {
            services.AddTransient<IOAuthResponseMode, QueryResponseModeHandler>();
            services.AddTransient<IOAuthResponseModeHandler, QueryResponseModeHandler>();
            services.AddTransient<IOAuthResponseMode, FragmentResponseModeHandler>();
            services.AddTransient<IOAuthResponseModeHandler, FragmentResponseModeHandler>();
            services.AddTransient<IResponseModeHandler, ResponseModeHandler>();
            return services;
        }

        private static IServiceCollection AddOAuthStore(this IServiceCollection services)
        {
            services.AddSingleton<IJsonWebKeyQueryRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultJsonWebKeyQueryRepository(opt.Value.DefaultJsonWebKeys);
            });
            services.AddSingleton<IJsonWebKeyCommandRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultJsonWebKeyCommandRepository(opt.Value.DefaultJsonWebKeys);
            });
            services.AddSingleton<IOAuthClientQueryRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultOAuthClientRepository(opt.Value.DefaultOAuthClients);
            });
            services.AddSingleton<IOAuthClientCommandRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultOAuthClientCommandRepository(opt.Value.DefaultOAuthClients);
            });
            services.AddSingleton<IOAuthUserQueryRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultOAuthUserQueryRepository(opt.Value.DefaultUsers);
            });
            services.AddSingleton<IOAuthUserCommandRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultOAuthUserCommandRepository(opt.Value.DefaultUsers);
            });
            services.AddSingleton<IOAuthScopeQueryRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultOAuthScopeQueryRepository(opt.Value.DefaultOAuthScopes);
            });
            services.AddSingleton<IOAuthScopeCommandRepository>(o =>
            {
                var opt = o.GetService<IOptions<OAuthHostOptions>>();
                return new DefaultOAuthScopeCommandRepository(opt.Value.DefaultOAuthScopes);
            });
            return services;
        }

        private static IServiceCollection AddOAuthClientAuthentication(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticateClient>(s => new AuthenticateClient(s.GetService<IJwsGenerator>(), s.GetService<IJwtParser>(),
                s.GetService<IOAuthClientQueryRepository>(), s.GetServices<IOAuthClientAuthenticationHandler>() ));
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientPrivateKeyJwtAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretBasicAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretJwtAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientSecretPostAuthenticationHandler>();
            services.AddTransient<IOAuthClientAuthenticationHandler, OAuthClientTlsAuthenticationHandler>();
            return services;
        }

        private static IServiceCollection AddOAuthJwksApi(this IServiceCollection services)
        {
            services.AddTransient<IJwksRequestHandler, JwksRequestHandler>();
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
            services.AddTransient<IJwtParser, JwtParser>();
            return services;
        }

        private static IServiceCollection AddOAuthService(this IServiceCollection services)
        {
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
            return services;
        }

        private static IServiceCollection AddRegisterApi(this IServiceCollection services)
        {
            services.AddTransient<IRegisterRequestHandler, OAuthRegisterRequestHandler>();
            return services;
        }
    }
}