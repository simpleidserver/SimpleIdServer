// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
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
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.OAuth.Persistence.Users;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register OAUTH2.0 dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerOAuthBuilder AddSIDOAuth(this IServiceCollection services)
        {
            services.AddMvc();
            services.AddDataProtection();
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
            return new SimpleIdServerOAuthBuilder(services);
        }

        /// <summary>
        /// Register OAUTH2.0 dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SimpleIdServerOAuthBuilder AddSIDOAuth(this IServiceCollection services, Action<OAuthHostOptions> options)
        {
            services.Configure(options);
            return services.AddSIDOAuth();
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
            var jwsGenerator = new JwsGeneratorFactory().BuildJwsGenerator();
            JsonWebKey sigJsonWebKey;
            JsonWebKey encJsonWebKey;
            using (var rsa = RSA.Create())
            {
                sigJsonWebKey = new JsonWebKeyBuilder().NewSign("1", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, "RS256").Build();
            }

            using (var rsa = RSA.Create())
            {
                encJsonWebKey = new JsonWebKeyBuilder().NewEnc("2", new[]
                {
                    KeyOperations.Encrypt,
                    KeyOperations.Decrypt
                }).SetAlg(rsa, RSAOAEPCEKHandler.ALG_NAME).Build();
            }

            var jsonWebKeys = new List<JsonWebKey>
            {
                sigJsonWebKey,
                encJsonWebKey
            };
            var clients = new List<OAuthClient>();
            var users = new List<OAuthUser>();
            var scopes = new List<OAuthScope>();
            services.TryAddSingleton<IJsonWebKeyQueryRepository>(new DefaultJsonWebKeyQueryRepository(jsonWebKeys));
            services.TryAddSingleton<IJsonWebKeyCommandRepository>(new DefaultJsonWebKeyCommandRepository(jsonWebKeys));
            services.TryAddSingleton<IOAuthClientQueryRepository>(new DefaultOAuthClientQueryRepository(clients));
            services.TryAddSingleton<IOAuthClientCommandRepository>(new DefaultOAuthClientCommandRepository(clients));
            services.TryAddSingleton<IOAuthUserQueryRepository>(new DefaultOAuthUserQueryRepository(users));
            services.TryAddSingleton<IOAuthUserCommandRepository>(new DefaultOAuthUserCommandRepository(users));
            services.TryAddSingleton<IOAuthScopeQueryRepository>(new DefaultOAuthScopeQueryRepository(scopes));
            services.TryAddSingleton<IOAuthScopeCommandRepository>(new DefaultOAuthScopeCommandRepository(scopes));
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