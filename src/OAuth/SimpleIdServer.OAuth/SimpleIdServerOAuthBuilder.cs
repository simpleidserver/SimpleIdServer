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

        public IServiceCollection Services => _serviceCollection;

        public SimpleIdServerOAuthBuilder AddUsers(List<OAuthUser> users )
        {
            _serviceCollection.AddSingleton<IOAuthUserRepository>(new DefaultOAuthUserRepository(users));
            return this;
        }

        public SimpleIdServerOAuthBuilder AddClients(List<OAuthClient> clients)
        {
            _serviceCollection.AddSingleton<IOAuthClientRepository>(new DefaultOAuthClientRepository(clients));
            return this;
        }

        public SimpleIdServerOAuthBuilder AddScopes(List<OAuthScope> scopes)
        {
            _serviceCollection.AddSingleton<IOAuthScopeRepository>(new DefaultOAuthScopeRepository(scopes));
            return this;
        }

        public SimpleIdServerOAuthBuilder AddJsonWebKeys(List<JsonWebKey> jwks)
        {
            _serviceCollection.AddSingleton<IJsonWebKeyRepository>(new DefaultJsonWebKeyRepository(jwks));
            return this;
        }
    }
}
