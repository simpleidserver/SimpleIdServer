// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim
{
    public class SimpleIdServerSCIMBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SimpleIdServerSCIMBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get => _serviceCollection; }

        public SimpleIdServerSCIMBuilder AddSchemas(List<SCIMSchema> schemas)
        {
            _serviceCollection.AddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
            _serviceCollection.AddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
            return this;
        }

        public SimpleIdServerSCIMBuilder AddAuthentication(Action<AuthenticationBuilder> callback)
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var scimHostOptions = serviceProvider.GetService<IOptionsMonitor<SCIMHostOptions>>().CurrentValue;
            var authenticationBuilder = _serviceCollection.AddAuthentication(scimHostOptions.AuthenticationScheme);
            callback(authenticationBuilder);
            return this;
        }
    }
}
