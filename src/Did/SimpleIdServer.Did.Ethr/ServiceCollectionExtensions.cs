// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDIDEthr(this IServiceCollection services)
        {
            // services.Configure<DidEthrOptions>(callback == null ? (o) => { } : callback);
            // services.AddTransient<IIdentityDocumentIdentifierParser, IdentityDocumentIdentifierParser>();
            // services.AddTransient<IIdentityDocumentExtractor, IdentityDocumentExtractor>();
            // services.AddTransient<ISmartContractServiceFactory, SmartContractServiceFactory>();
            // services.AddTransient<IDIDRegistryServiceFactory, DIDRegistryServiceFactory>();
            // services.AddTransient<IDIDGenerator, DIDEthrGenerator>();
            // services.AddTransient<IContractDeploy, EthrContractDeploy>();
            return services;
        }
    }
}
