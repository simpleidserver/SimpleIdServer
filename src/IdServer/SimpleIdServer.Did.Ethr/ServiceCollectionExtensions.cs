// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did;
using SimpleIdServer.Did.Ethr;
using SimpleIdServer.Did.Ethr.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDIDEthr(this IServiceCollection services, DidEthrOptions options)
        {
            services.AddSingleton(options ?? new DidEthrOptions());
            services.AddTransient<IIdentityDocumentIdentifierParser, IdentityDocumentIdentifierParser>();
            services.AddTransient<IIdentityDocumentExtractor, IdentityDocumentExtractor>();
            services.AddSingleton<IIdentityDocumentConfigurationStore, IdentityDocumentConfigurationStore>();
            services.AddTransient<IDIDRegistryServiceFactory, DIDRegistryServiceFactory>();
            return services;
        }
    }
}
