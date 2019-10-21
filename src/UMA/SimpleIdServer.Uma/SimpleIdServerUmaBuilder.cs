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
    }
}
