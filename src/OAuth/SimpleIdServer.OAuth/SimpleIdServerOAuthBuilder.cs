// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.InMemory;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth
{
    public class SimpleIdServerOAuthBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SimpleIdServerOAuthBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public SimpleIdServerOAuthBuilder AddUsers(List<OAuthUser> users )
        {
            _serviceCollection.AddSingleton<IOAuthUserQueryRepository>(new DefaultOAuthUserQueryRepository(users));
            _serviceCollection.AddSingleton<IOAuthUserCommandRepository>(new DefaultOAuthUserCommandRepository(users));
            return this;
        }

        public SimpleIdServerOAuthBuilder AddClients(List<OAuthClient> clients)
        {
            _serviceCollection.AddSingleton<IOAuthClientQueryRepository>(new DefaultOAuthClientQueryRepository(clients));
            _serviceCollection.AddSingleton<IOAuthClientCommandRepository>(new DefaultOAuthClientCommandRepository(clients));
            return this;
        }

        public SimpleIdServerOAuthBuilder AddScopes(List<OAuthScope> scopes)
        {
            _serviceCollection.AddSingleton<IOAuthScopeQueryRepository>(new DefaultOAuthScopeQueryRepository(scopes));
            _serviceCollection.AddSingleton<IOAuthScopeCommandRepository>(new DefaultOAuthScopeCommandRepository(scopes));
            return this;
        }

        public SimpleIdServerOAuthBuilder AddJsonWebKeys(List<JsonWebKey> jwks)
        {
            _serviceCollection.AddSingleton<IJsonWebKeyQueryRepository>(new DefaultJsonWebKeyQueryRepository(jwks));
            _serviceCollection.AddSingleton<IJsonWebKeyCommandRepository>(new DefaultJsonWebKeyCommandRepository(jwks));
            return this;
        }
    }
}
