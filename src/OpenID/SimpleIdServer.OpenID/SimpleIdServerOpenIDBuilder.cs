// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.OpenID.ClaimsEnrichers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Persistence;
using SimpleIdServer.OpenID.Persistence.InMemory;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID
{
    public class SimpleIdServerOpenIDBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SimpleIdServerOpenIDBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get => _serviceCollection; }

        public SimpleIdServerOpenIDBuilder AddUsers(List<OAuthUser> users)
        {
            _serviceCollection.AddSingleton<IOAuthUserQueryRepository>(new DefaultOAuthUserQueryRepository(users));
            _serviceCollection.AddSingleton<IOAuthUserCommandRepository>(new DefaultOAuthUserCommandRepository(users));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddClients(List<OpenIdClient> clients, List<OpenIdScope> scopes)
        {
            _serviceCollection.AddSingleton<IOAuthClientQueryRepository>(new DefaultOpenIdClientQueryRepository(clients));
            _serviceCollection.AddSingleton<IOAuthClientCommandRepository>(new DefaultOpenIdClientCommandRepository(clients, scopes));
            _serviceCollection.AddSingleton<IOAuthScopeQueryRepository>(new DefaultOpenIdScopeQueryRepository(scopes));
            _serviceCollection.AddSingleton<IOAuthScopeCommandRepository>(new DefaultOpenIdScopeCommandRepository(scopes));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddJsonWebKeys(List<JsonWebKey> jwks)
        {
            _serviceCollection.AddSingleton<IJsonWebKeyQueryRepository>(new DefaultJsonWebKeyQueryRepository(jwks));
            _serviceCollection.AddSingleton<IJsonWebKeyCommandRepository>(new DefaultJsonWebKeyCommandRepository(jwks));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddAcrs(List<AuthenticationContextClassReference> acrs)
        {
            _serviceCollection.AddSingleton<IAuthenticationContextClassReferenceCommandRepository>(new DefaultAuthenticationContextClassReferenceCommandRepository(acrs));
            _serviceCollection.AddSingleton<IAuthenticationContextClassReferenceQueryRepository>(new DefaultAuthenticationContextClassReferenceQueryRepository(acrs));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddAggregateHttpClaimsSource(AggregateHttpClaimsSourceOptions httpClaimsSourceOptions)
        {
            _serviceCollection.AddTransient<IClaimsSource, AggregateHttpClaimsSource>(o => new AggregateHttpClaimsSource(httpClaimsSourceOptions, o.GetService<IJwtBuilder>(), o.GetService<IJwtParser>()));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddDistributeHttpClaimsSource(DistributeHttpClaimsSourceOptions distributeHttpClaimsSourceOptions)
        {
            _serviceCollection.AddTransient<IClaimsSource, DistributeHttpClaimsSource>(o => new DistributeHttpClaimsSource(distributeHttpClaimsSourceOptions));
            return this;
        }
    }
}
