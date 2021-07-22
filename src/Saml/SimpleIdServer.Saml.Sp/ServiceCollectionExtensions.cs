// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Sp;
using SimpleIdServer.Saml.Sp.Apis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamlSp(this IServiceCollection serviceCollection, Action<SamlSpOptions> callback = null)
        {
            serviceCollection.AddMetadataApi();
            if (callback == null)
            {
                serviceCollection.Configure<SamlSpOptions>((opt) => { });
            } 
            else
            {
                serviceCollection.Configure(callback);
            }

            return serviceCollection;
        }

        private static IServiceCollection AddMetadataApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMetadataHandler, MetadataHandler>();
            return serviceCollection;
        }
    }
}
