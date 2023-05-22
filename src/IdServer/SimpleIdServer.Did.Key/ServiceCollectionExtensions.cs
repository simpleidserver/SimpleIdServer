// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did;
using SimpleIdServer.Did.Key;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDIDKey(this IServiceCollection services, DidKeyOptions keyOptions = null)
        {
            services.AddSingleton(keyOptions ?? new DidKeyOptions());
            services.AddTransient<IIdentityDocumentIdentifierParser, IdentityDocumentIdentifierParser>();
            services.AddTransient<IIdentityDocumentExtractor, IdentityDocumentExtractor>();
            return services;
        }
    }
}
