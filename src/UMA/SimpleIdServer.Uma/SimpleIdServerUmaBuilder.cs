// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.Persistence;
using SimpleIdServer.Uma.Persistence.InMemory;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Uma
{
    public class SimpleIdServerUmaBuilder : SimpleIdServerOAuthBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SimpleIdServerUmaBuilder(IServiceCollection serviceCollection) : base(serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get => _serviceCollection; }

        public SimpleIdServerUmaBuilder AddUmaResources(List<UMAResource> umaResources)
        {
            _serviceCollection.AddSingleton<IUMAResourceCommandRepository>(new DefaultUMAResourceCommandRepository(umaResources));
            _serviceCollection.AddSingleton<IUMAResourceQueryRepository>(new DefaultUMAResourceQueryRepository(umaResources));
            return this;
        }        
        
        public SimpleIdServerUmaBuilder AddUMARequests(List<UMAPendingRequest> umaPendingRequests)
        {
            _serviceCollection.AddSingleton<IUMAPendingRequestCommandRepository>(new DefaultUMAPendingRequestCommandRepository(umaPendingRequests));
            _serviceCollection.AddSingleton<IUMAPendingRequestQueryRepository>(new DefaultUMAPendingRequestQueryRepository(umaPendingRequests));
            return this;
        }

        public SimpleIdServerUmaBuilder AddAuthentication(Action<AuthenticationBuilder> callback)
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var umaHostOptions = serviceProvider.GetService<IOptionsMonitor<UMAHostOptions>>().CurrentValue;
            var authenticationBuilder = _serviceCollection.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = umaHostOptions.SignInScheme;
                opts.DefaultSignInScheme = umaHostOptions.SignInScheme;
                opts.DefaultChallengeScheme = umaHostOptions.ChallengeAuthenticationScheme;
            });
            callback(authenticationBuilder);
            return this;
        }
    }
}
