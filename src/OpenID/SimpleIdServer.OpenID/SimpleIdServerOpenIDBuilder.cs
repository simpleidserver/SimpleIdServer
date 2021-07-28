// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
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
using SimpleIdServer.OpenID.UI.AuthProviders;
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
            _serviceCollection.AddSingleton<IOAuthUserRepository>(new DefaultOAuthUserRepository(users));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddClients(List<OpenIdClient> clients, List<OAuthScope> scopes)
        {
            _serviceCollection.AddSingleton<IOAuthClientRepository>(new DefaultOpenIdClientRepository(clients));
            _serviceCollection.AddSingleton<IOAuthScopeRepository>(new DefaultOpenIdScopeRepository(scopes));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddJsonWebKeys(List<JsonWebKey> jwks)
        {
            _serviceCollection.AddSingleton<IJsonWebKeyRepository>(new DefaultJsonWebKeyRepository(jwks));
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddAcrs(List<AuthenticationContextClassReference> acrs)
        {
            _serviceCollection.AddSingleton<IAuthenticationContextClassReferenceRepository>(new DefaultAuthenticationContextClassReferenceRepository(acrs));
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

        public SimpleIdServerOpenIDBuilder AddDynamicAuthenticationProviders()
        {
            _serviceCollection.AddTransient<IAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
            _serviceCollection.AddTransient<ISIDAuthenticationSchemeProvider, DynamicAuthenticationSchemeProvider>();
            _serviceCollection.AddTransient<IAuthenticationHandlerProvider, DynamicAuthenticationHandlerProvider>();
            return this;
        }

        public SimpleIdServerOpenIDBuilder AddAuthenticationProviderSchemes(ICollection<Domains.AuthenticationSchemeProvider> authenticationSchemes)
        {
            _serviceCollection.AddSingleton<IAuthenticationSchemeProviderRepository>(new InMemoryAuthenticationSchemeProviderRepository(authenticationSchemes));
            return this;
        }
    }
}
