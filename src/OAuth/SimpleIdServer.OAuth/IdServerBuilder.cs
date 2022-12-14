// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Stores;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IdServerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceProvider _serviceProvider;

        public IdServerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public IServiceCollection Services => _serviceCollection;
        public IServiceProvider ServiceProvider => _serviceProvider;

        public IdServerBuilder AddSigningKey(SigningCredentials signingCredentials)
        {
            _serviceCollection.AddSingleton<IKeyStore>(new InMemoryKeyStore(signingCredentials));
            return this;
        }

        public IdServerBuilder AddSigningKey(RsaSecurityKey rsa, string signingAlg = SecurityAlgorithms.RsaSha256)
        {
            var signingCredentials = new SigningCredentials(rsa, signingAlg);
            return AddSigningKey(signingCredentials);
        }

        public IdServerBuilder AddInMemoryScopes(ICollection<Scope> scopes)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            storeDbContext.Scopes.AddRange(scopes);
            storeDbContext.SaveChanges();
            return this;
        }
    }
}
