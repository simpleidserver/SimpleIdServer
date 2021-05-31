// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OAuth;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.Persistence;
using SimpleIdServer.Uma.Persistence.InMemory;
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
            _serviceCollection.AddSingleton<IUMAResourceRepository>(new DefaultUMAResourceRepository(umaResources));
            return this;
        }        
        
        public SimpleIdServerUmaBuilder AddUMARequests(List<UMAPendingRequest> umaPendingRequests)
        {
            _serviceCollection.AddSingleton<IUMAPendingRequestRepository>(new DefaultUMAPendingRequestRepository(umaPendingRequests));
            return this;
        }
    }
}
